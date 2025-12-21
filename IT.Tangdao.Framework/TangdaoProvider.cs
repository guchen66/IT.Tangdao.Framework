using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using IT.Tangdao.Framework.Ioc;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// 默认只读解析器，线程安全由策略与工厂保证。
    /// </summary>
    public sealed class TangdaoProvider : ITangdaoProvider
    {
        private readonly IServiceRegistry _registry;
        private readonly IServiceFactory _factory;

        internal TangdaoProvider(IServiceRegistry registry, IServiceFactory factory)
        {
            _registry = registry;
            _factory = factory;
            // _snapshot = registry.GetAllEntries(); // 建造阶段一次性拍快照，后续线程安全遍
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            var entry = _registry.GetEntry(serviceType);
            if (entry == null) return null;

            // 把缓存字典作为额外参数传给策略，由策略决定是否使用
            return entry.LifecycleStrategy.CreateInstance(
                new ServiceCreationContext() { Entry = entry, Factory = _factory });
        }

        public T GetService<T>() where T : class
            => GetService(typeof(T)) as T;

        public object GetKeyedService(Type serviceType, object key)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (key == null) throw new ArgumentNullException(nameof(key));

            // 1. 只走新字典，不影响旧字典
            var entry = (_registry as ServiceRegistry)?.GetKeyedEntry(serviceType, key);
            if (entry == null) return null;

            // 2. 生命周期策略与 GetService 共用同一套，无差别
            return entry.LifecycleStrategy.CreateInstance(
                new ServiceCreationContext { Entry = entry, Factory = _factory, Key = key });
        }

        public T GetKeyedService<T>(object key) where T : class
            => GetKeyedService(typeof(T), key) as T;
    }
}