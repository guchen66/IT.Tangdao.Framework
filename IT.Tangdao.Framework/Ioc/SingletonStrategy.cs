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
        // 普通单例缓存（保持原样）
        private static readonly ConcurrentDictionary<Type, object> _cache =
            new ConcurrentDictionary<Type, object>();

        // 新增：带键单例缓存
        private static readonly ConcurrentDictionary<string, object> _keyedCache =
            new ConcurrentDictionary<string, object>();

        public object CreateInstance(ServiceCreationContext context)
        {
            if (context.Key == null)
            {
                // 普通单例：使用原有逻辑
                return _cache.GetOrAdd(context.Entry.ServiceType,
                    _ => context.Factory.Create(context.Entry));
            }
            else
            {
                // 带键单例：生成唯一的缓存键
                string cacheKey = GenerateCacheKey(context.Entry.ServiceType, context.Key);
                return _keyedCache.GetOrAdd(cacheKey,
                    _ => context.Factory.Create(context.Entry));
            }
        }

        private static string GenerateCacheKey(Type serviceType, object key)
        {
            // 使用分隔符创建唯一的字符串键
            // 例如："ITangdaoPage||RelaPathViewModel"
            return $"{serviceType.FullName}||{key}";
        }
    }
}