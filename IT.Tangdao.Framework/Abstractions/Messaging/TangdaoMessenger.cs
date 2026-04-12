using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Exceptions;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.Abstractions.Messaging
{
    /// <summary>
    /// 消息传递者，实现单例模式
    /// 负责管理消息观察者的注册、注销和消息分发
    /// </summary>
    public class TangdaoMessenger
    {
        /// <summary>
        /// 懒加载单例实例
        /// </summary>
        private static readonly Lazy<TangdaoMessenger> _instance = new Lazy<TangdaoMessenger>(() => new TangdaoMessenger());

        /// <summary>
        /// 获取消息传递者的单例实例
        /// </summary>
        public static TangdaoMessenger Instance => _instance.Value;

        /// <summary>
        /// 观察者缓存实例，用于创建观察者
        /// </summary>
        internal IObserverCache ObserverCache { get; } = new ObserverCache();

        /// <summary>
        /// 观察者列表，存储所有注册的消息观察者
        /// </summary>
        private readonly List<IMessageObserver> _observers = new List<IMessageObserver>();

        /// <summary>
        /// Key到观察者实例的映射表，用于通过Key快速查找观察者
        /// </summary>
        private readonly Dictionary<string, IMessageObserver> _keyToObserverMap = new Dictionary<string, IMessageObserver>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 线程同步锁，保护观察者列表和映射表的并发访问
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 静态线程同步锁，保护静态资源的并发访问
        /// </summary>
        private static readonly object _staticLock = new object();

        /// <summary>
        /// 当消息状态改变时触发
        /// </summary>
        public event Action<MessageContext> MessageChanged;

        /// <summary>
        /// 服务解析器委托，用于创建消息观察者实例
        /// 委托形状：给我 TypeEntry，我还你实例
        /// </summary>
        internal static Func<IRegistrationTypeEntry, IMessageObserver> ServiceResolver { get; private set; }
            = reg => DefaultResolve(reg);

        /// <summary>
        /// 默认服务解析实现，通过内置容器获取服务实例
        /// </summary>
        /// <param name="reg">消息注册表，包含观察者类型信息</param>
        /// <returns>创建的消息观察者实例，如果创建失败则返回null</returns>
        private static IMessageObserver DefaultResolve(IRegistrationTypeEntry reg)
            => ServiceLocator.Default.GetService(reg.RegisterType) as IMessageObserver;

        /// <summary>
        /// 设置服务解析器，用于自定义观察者实例的创建逻辑
        /// 一次性注入点，只能设置一次
        /// </summary>
        /// <param name="resolver">自定义的服务解析器委托</param>
        /// <exception cref="ArgumentNullException">当resolver为null时抛出</exception>
        public void SetResolver(Func<IRegistrationTypeEntry, IMessageObserver> resolver)
        {
            if (resolver == null) TangdaoGuards.ThrowIfNull(nameof(resolver));
            lock (_staticLock)
                ServiceResolver = resolver;
        }

        /// <summary>
        /// 私有构造函数，实现单例模式
        /// </summary>
        private TangdaoMessenger()
        { }

        /// <summary>
        /// 设置自定义的消息观察者解析器
        /// 允许外部注入自定义的IObserverCache实现，提高依赖注入支持
        /// </summary>
        /// <param name="resolver">自定义的IObserverCache实例</param>
        /// <exception cref="ArgumentNullException">当resolver为null时抛出</exception>
        public void SetResolver(IObserverCache resolver)
        {
            if (resolver == null) TangdaoGuards.ThrowIfNull(nameof(resolver));

            // 创建一个包装器，将IObserverCache实例转换为ServiceResolver委托
            SetResolver(reg => resolver.CreateObserver(reg));
        }

        /// <summary>
        /// 注册消息观察者
        /// </summary>
        /// <param name="key">观察者的Key，用于后续通过Key查找和通知</param>
        /// <param name="observer">要注册的消息观察者实例</param>
        internal void Register(string key, IMessageObserver observer)
        {
            if (observer == null) TangdaoGuards.ThrowIfNull(nameof(observer));

            lock (_lock)
            {
                bool isNew = !_observers.Contains(observer);
                if (isNew)
                {
                    _observers.Add(observer);
                }

                // 更新Key到观察者的映射
                if (!string.IsNullOrWhiteSpace(key))
                {
                    _keyToObserverMap[key] = observer;
                }
            }
        }

        /// <summary>
        /// 注销消息观察者
        /// </summary>
        /// <param name="observer">要注销的消息观察者实例</param>
        /// <exception cref="ArgumentNullException">当observer为null时抛出</exception>
        public void Unregister(IMessageObserver observer)
        {
            if (observer == null) TangdaoGuards.ThrowIfNull(nameof(observer));

            lock (_lock)
            {
                bool removed = _observers.Remove(observer);
                if (removed)
                {
                    // 从映射表中移除所有指向该观察者的Key映射
                    var keysToRemove = _keyToObserverMap.Where(kv => kv.Value == observer).Select(kv => kv.Key).ToList();
                    foreach (var key in keysToRemove)
                    {
                        _keyToObserverMap.Remove(key);
                    }

                    // 清理NoticeResolver中的缓存
                    ObserverCache.RemoveFromCache(observer.GetType());
                }
            }
        }

        /// <summary>
        /// 通过Key注销通知观察者
        /// </summary>
        /// <param name="key">要注销的观察者的Key</param>
        /// <exception cref="ArgumentNullException">当key为null或空字符串时抛出</exception>
        public void Unregister(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) TangdaoGuards.ThrowIfNull(nameof(key));

            lock (_lock)
            {
                if (_keyToObserverMap.TryGetValue(key, out var observer))
                {
                    _observers.Remove(observer);
                    _keyToObserverMap.Remove(key);

                    // 清理NoticeResolver中的缓存
                    ObserverCache.RemoveFromCache(observer.GetType());
                }
            }
        }

        /// <summary>
        /// 一键注销所有通知观察者
        /// 清空所有观察者列表、映射表和缓存，释放资源
        /// 适用于应用关闭、系统重置或批量清理场景
        /// </summary>
        public void Unregister()
        {
            lock (_lock)
            {
                int count = _observers.Count;
                if (count > 0)
                {
                    // 清空观察者列表
                    _observers.Clear();
                    // 清空Key到观察者的映射
                    _keyToObserverMap.Clear();
                    // 清空NoticeResolver中的缓存
                    ObserverCache.ClearCache();
                }
            }
        }

        /// <summary>
        /// 消息状态变化，向所有注册的观察者发送消息
        /// 同时触发MessageChanged事件，方便UI层监听
        /// </summary>
        /// <param name="context">消息上下文，包含消息的相关信息</param>
        public void NotifyStateChange(MessageContext context)
        {
            InternalNotifyObservers(context);
            // 使用本地变量缓存事件委托，避免多线程下的竞态条件
            var message = MessageChanged;
            message?.Invoke(context); // 让 UI 层也能监听
        }

        /// <summary>
        /// 向所有注册的观察者发送消息
        /// </summary>
        /// <param name="context">消息上下文，包含消息的相关信息</param>
        public void NotifyObservers(MessageContext context)
        {
            InternalNotifyObservers(context);
        }

        /// <summary>
        /// 向所有注册的观察者发送消息（私有辅助方法）
        /// </summary>
        /// <param name="context">消息上下文，包含消息的相关信息</param>
        private void InternalNotifyObservers(MessageContext context)
        {
            if (context == null) TangdaoGuards.ThrowIfNull(nameof(context));

            List<IMessageObserver> observersCopy;
            lock (_lock)
            {
                observersCopy = new List<IMessageObserver>(_observers);
            }

            foreach (var observer in observersCopy)
            {
                if (observer.IsReceive)
                {
                    MessageEventArgs messageEventArgs = new MessageEventArgs(context);
                    observer.MessageIntercepted?.Invoke(observer, messageEventArgs);
                    observer.UpdateMessage(context);
                }
            }
        }

        /// <summary>
        /// 向指定的单个观察者发送消息
        /// </summary>
        /// <param name="observer">要通知的观察者实例</param>
        /// <param name="context">消息上下文，包含消息的相关信息</param>
        /// <exception cref="ArgumentNullException">当observer或context为null时抛出</exception>
        public void NotifyObserver(IMessageObserver observer, MessageContext context)
        {
            if (observer == null) TangdaoGuards.ThrowIfNull(nameof(observer));
            if (context == null) TangdaoGuards.ThrowIfNull(nameof(context));

            if (observer.IsReceive)
            {
                MessageEventArgs messageEventArgs = new MessageEventArgs(context);
                observer.MessageIntercepted?.Invoke(observer, messageEventArgs);
                observer.UpdateMessage(context);
            }
        }

        /// <summary>
        /// 根据条件筛选第一个匹配的观察者并发送消息
        /// </summary>
        /// <param name="predicate">筛选观察者的条件委托</param>
        /// <param name="context">消息上下文，包含消息的相关信息</param>
        /// <exception cref="ArgumentNullException">当predicate或context为null时抛出</exception>
        public void NotifyFirstObserver(Func<IMessageObserver, bool> predicate, MessageContext context)
        {
            if (predicate == null) TangdaoGuards.ThrowIfNull(nameof(predicate));
            if (context == null) TangdaoGuards.ThrowIfNull(nameof(context));

            List<IMessageObserver> observersCopy;
            lock (_lock)
            {
                observersCopy = new List<IMessageObserver>(_observers);
            }

            var targetObserver = observersCopy.FirstOrDefault(predicate); // 锁外执行
            if (targetObserver != null)
            {
                if (targetObserver.IsReceive)
                {
                    MessageEventArgs messageEventArgs = new MessageEventArgs(context);
                    targetObserver.MessageIntercepted?.Invoke(targetObserver, messageEventArgs);
                    targetObserver.UpdateMessage(context);
                }
            }
            // 如果找不到匹配的观察者，静默处理（与NotifyObserverByKey保持一致）
        }

        /// <summary>
        /// 根据条件筛选所有匹配的观察者并发送消息
        /// </summary>
        /// <param name="predicate">筛选观察者的条件委托</param>
        /// <param name="context">消息上下文，包含消息的相关信息</param>
        public void NotifyObservers(Func<IMessageObserver, bool> predicate, MessageContext context)
        {
            if (predicate == null) TangdaoGuards.ThrowIfNull(nameof(predicate));
            if (context == null) TangdaoGuards.ThrowIfNull(nameof(context));

            List<IMessageObserver> observersCopy;
            lock (_lock)
            {
                observersCopy = new List<IMessageObserver>(_observers);
            }

            // 直接遍历快照，不额外创建过滤后的列表
            foreach (var observer in observersCopy)
            {
                if (predicate(observer) && observer.IsReceive)
                {
                    MessageEventArgs messageEventArgs = new MessageEventArgs(context);
                    observer.MessageIntercepted?.Invoke(observer, messageEventArgs);
                    observer.UpdateMessage(context);
                }
            }
        }

        /// <summary>
        /// 通过Key向单个观察者发送消息
        /// </summary>
        /// <param name="key">观察者的Key，用于查找对应的观察者实例</param>
        /// <param name="context">消息上下文，包含消息的相关信息</param>
        /// <exception cref="ArgumentNullException">当key或context为null时抛出</exception>
        public void NotifyObserverByKey(string key, MessageContext context)
        {
            if (string.IsNullOrWhiteSpace(key)) TangdaoGuards.ThrowIfNull(nameof(key));
            if (context == null) TangdaoGuards.ThrowIfNull(nameof(context));

            lock (_lock)
            {
                if (_keyToObserverMap.TryGetValue(key, out var observer))
                {
                    if (observer.IsReceive)
                    {
                        MessageEventArgs messageEventArgs = new MessageEventArgs(context);
                        observer.MessageIntercepted?.Invoke(observer, messageEventArgs);
                        observer.UpdateMessage(context);
                    }
                }
                // 如果找不到对应的观察者，这里可以选择抛出异常或者静默处理
                // 考虑到消息系统的特性，静默处理可能更合适
            }
        }
    }
}