using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 单例策略：第一次创建后存入 ConcurrentDictionary，后续复用。
    /// 线程安全由字典保证。
    /// </summary>
    public sealed class SingletonStrategy : ILifecycleStrategy
    {
        // 静态全局缓存，也可改成 Provider 级实例缓存，后续再换
        private static readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        public object CreateInstance(ServiceCreationContext context)
        {
            return _cache.GetOrAdd(context.Entry.ServiceType,
                _ => context.Factory.Create(context.Entry));
        }
    }
}