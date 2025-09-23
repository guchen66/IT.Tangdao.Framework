using System;
using System.Collections.Concurrent;
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

        // Singleton 实例缓存（线程安全字典）
        private readonly ConcurrentDictionary<Type, object> _singletonCache = new ConcurrentDictionary<Type, object>();

        internal TangdaoProvider(IServiceRegistry registry, IServiceFactory factory)
        {
            _registry = registry;
            _factory = factory;
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
    }
}