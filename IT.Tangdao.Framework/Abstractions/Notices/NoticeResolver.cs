using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Extensions;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知观察者解析器，实现了INoticeResolver接口
    /// 负责根据通知注册表创建通知观察者实例
    /// 内部使用缓存机制避免重复创建相同类型的观察者实例
    /// </summary>
    public sealed class NoticeResolver : INoticeResolver
    {
        /// <summary>
        /// 观察者实例缓存，用于存储已经创建的观察者实例
        /// Key: 观察者类型的完整名称
        /// Value: 对应的观察者实例
        /// </summary>
        private readonly Dictionary<string, INoticeObserver> _observerCache = new Dictionary<string, INoticeObserver>();

        /// <summary>
        /// 线程同步锁，保护缓存的并发访问
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 根据通知注册表创建通知观察者实例
        /// </summary>
        /// <param name="noticeRegistry">通知注册表，包含观察者类型信息</param>
        /// <returns>创建的通知观察者实例，如果创建失败则返回null</returns>
        /// <exception cref="ArgumentNullException">当noticeRegistry为null时抛出</exception>
        public INoticeObserver CreateObserver(NoticeRegistry noticeRegistry)
        {
            if (noticeRegistry == null) throw new ArgumentNullException(nameof(noticeRegistry));

            // 获取缓存键：使用类型的完整名称作为缓存键
            string cacheKey = noticeRegistry.RegisterType.FullName;

            // 先检查缓存中是否已经存在该观察者实例
            INoticeObserver observer;
            lock (_lock)
            {
                if (_observerCache.TryGetValue(cacheKey, out observer))
                {
                    return observer;
                }
            }

            // 缓存中不存在，创建新实例
            observer = NoticeMediator.ServiceResolver(noticeRegistry);

            // 如果创建成功，添加到缓存中
            if (observer != null)
            {
                lock (_lock)
                {
                    // 双重检查锁定，避免并发情况下重复创建
                    if (!_observerCache.ContainsKey(cacheKey))
                    {
                        _observerCache[cacheKey] = observer;
                    }
                }

                return observer;
            }

            // 如果创建失败，返回默认值（null）
            return default;
        }

        /// <summary>
        /// 清除观察者实例缓存
        /// </summary>
        public void ClearCache()
        {
            lock (_lock)
            {
                _observerCache.Clear();
            }
        }

        /// <summary>
        /// 从缓存中移除指定类型的观察者实例
        /// </summary>
        /// <param name="observerType">观察者类型</param>
        /// <returns>如果移除成功则返回true，否则返回false</returns>
        public bool RemoveFromCache(Type observerType)
        {
            if (observerType == null) throw new ArgumentNullException(nameof(observerType));

            lock (_lock)
            {
                return _observerCache.Remove(observerType.FullName);
            }
        }
    }
}