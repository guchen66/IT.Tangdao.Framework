using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知指令的中介者，实现单例模式
    /// 负责管理通知观察者的注册、注销和通知分发
    /// </summary>
    public class NoticeMediator
    {
        /// <summary>
        /// 懒加载单例实例
        /// </summary>
        private static readonly Lazy<NoticeMediator> _instance = new Lazy<NoticeMediator>(() => new NoticeMediator());

        /// <summary>
        /// 日志记录器，用于记录通知系统的运行情况
        /// </summary>
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(NoticeMediator));

        /// <summary>
        /// 获取通知中介者的单例实例
        /// </summary>
        public static NoticeMediator Instance => _instance.Value;

        /// <summary>
        /// 通知解析器实例，用于创建观察者
        /// </summary>
        internal INoticeResolver Resolver { get; } = new NoticeResolver();

        /// <summary>
        /// 观察者列表，存储所有注册的通知观察者
        /// </summary>
        private readonly List<INoticeObserver> _observers = new List<INoticeObserver>();

        /// <summary>
        /// Key到观察者实例的映射表，用于通过Key快速查找观察者
        /// </summary>
        private readonly Dictionary<string, INoticeObserver> _keyToObserverMap = new Dictionary<string, INoticeObserver>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 线程同步锁，保护观察者列表和映射表的并发访问
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 静态线程同步锁，保护静态资源的并发访问
        /// </summary>
        private static readonly object _staticLock = new object();

        /// <summary>
        /// 值变化事件，当通知状态改变时触发
        /// </summary>
        public event Action<NoticeContext> ValueChanged;

        /// <summary>
        /// 服务解析器委托，用于创建通知观察者实例
        /// 委托形状：给我 RegistrationTypeEntry ，我还你实例
        /// </summary>
        internal static Func<RegistrationTypeEntry, INoticeObserver> ServiceResolver { get; private set; }
            = reg => DefaultResolve(reg);

        /// <summary>
        /// 默认服务解析实现，通过内置容器获取服务实例
        /// </summary>
        /// <param name="reg">通知注册表，包含观察者类型信息</param>
        /// <returns>创建的通知观察者实例，如果创建失败则返回null</returns>
        private static INoticeObserver DefaultResolve(RegistrationTypeEntry reg)
            => TangdaoApplication.Provider.GetService(reg.RegisterType) as INoticeObserver;

        /// <summary>
        /// 设置服务解析器，用于自定义观察者实例的创建逻辑
        /// 一次性注入点，只能设置一次
        /// </summary>
        /// <param name="resolver">自定义的服务解析器委托</param>
        /// <exception cref="ArgumentNullException">当resolver为null时抛出</exception>
        public static void SetResolver(Func<RegistrationTypeEntry, INoticeObserver> resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            lock (_staticLock)
                ServiceResolver = resolver;
        }

        /// <summary>
        /// 私有构造函数，实现单例模式
        /// </summary>
        private NoticeMediator()
        { }

        /// <summary>
        /// 设置自定义的通知观察者解析器
        /// 允许外部注入自定义的INoticeResolver实现，提高依赖注入支持
        /// </summary>
        /// <param name="resolver">自定义的INoticeResolver实例</param>
        /// <exception cref="ArgumentNullException">当resolver为null时抛出</exception>
        public void SetResolver(INoticeResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            // 创建一个包装器，将INoticeResolver实例转换为ServiceResolver委托
            SetResolver(reg => resolver.CreateObserver(reg));
        }

        /// <summary>
        /// 注册通知观察者
        /// </summary>
        /// <param name="key">观察者的Key，用于后续通过Key查找和通知</param>
        /// <param name="observer">要注册的通知观察者实例</param>
        internal void Register(string key, INoticeObserver observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            lock (_lock)
            {
                bool isNew = !_observers.Contains(observer);
                if (isNew)
                {
                    _observers.Add(observer);
                    Logger.Debug($"注册了新的通知观察者: {observer.GetType().FullName}，当前观察者数量: {_observers.Count}");
                }

                // 更新Key到观察者的映射
                if (!string.IsNullOrWhiteSpace(key))
                {
                    _keyToObserverMap[key] = observer;
                    Logger.Debug($"更新观察者映射: Key='{key}' -> Observer='{observer.GetType().FullName}'");
                }
            }
        }

        /// <summary>
        /// 注销通知观察者
        /// </summary>
        /// <param name="observer">要注销的通知观察者实例</param>
        /// <exception cref="ArgumentNullException">当observer为null时抛出</exception>
        public void Unregister(INoticeObserver observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

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
                    Resolver.RemoveFromCache(observer.GetType());
                    Logger.Debug($"成功注销观察者: {observer.GetType().FullName}，当前观察者数量: {_observers.Count}");
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
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            lock (_lock)
            {
                if (_keyToObserverMap.TryGetValue(key, out var observer))
                {
                    _observers.Remove(observer);
                    _keyToObserverMap.Remove(key);

                    // 清理NoticeResolver中的缓存
                    Resolver.RemoveFromCache(observer.GetType());
                    Logger.Debug($"成功通过Key注销观察者: {key} ({observer.GetType().FullName})，当前观察者数量: {_observers.Count}");
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
                    Resolver.ClearCache();

                    Logger.Debug($"成功注销所有观察者，共注销 {count} 个观察者");
                }
            }
        }

        /// <summary>
        /// 通知状态变化，向所有注册的观察者发送通知
        /// 同时触发ValueChanged事件，方便UI层监听
        /// </summary>
        /// <param name="context">通知上下文，包含通知的相关信息</param>
        public void NotifyStateChange(NoticeContext context)
        {
            NotifyObservers(context);
            // 使用本地变量缓存事件委托，避免多线程下的竞态条件
            var valueChanged = ValueChanged;
            valueChanged?.Invoke(context); // 让 UI 层也能监听
        }

        /// <summary>
        /// 向所有注册的观察者发送通知
        /// </summary>
        /// <param name="context">通知上下文，包含通知的相关信息</param>
        public void NotifyAll(NoticeContext context)
        {
            NotifyObservers(context);
        }

        /// <summary>
        /// 向所有注册的观察者发送通知（私有辅助方法）
        /// </summary>
        /// <param name="context">通知上下文，包含通知的相关信息</param>
        private void NotifyObservers(NoticeContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            List<INoticeObserver> observersCopy;
            lock (_lock)
            {
                observersCopy = new List<INoticeObserver>(_observers);
            }

            foreach (var observer in observersCopy)
            {
                observer.UpdateNotice(context);
            }
        }

        /// <summary>
        /// 向指定的单个观察者发送通知
        /// </summary>
        /// <param name="observer">要通知的观察者实例</param>
        /// <param name="context">通知上下文，包含通知的相关信息</param>
        /// <exception cref="ArgumentNullException">当observer或context为null时抛出</exception>
        public static void NotifySingle(INoticeObserver observer, NoticeContext context)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (context == null) throw new ArgumentNullException(nameof(context));

            observer.UpdateNotice(context);
        }

        /// <summary>
        /// 通过Key向单个观察者发送通知
        /// </summary>
        /// <param name="key">观察者的Key，用于查找对应的观察者实例</param>
        /// <param name="context">通知上下文，包含通知的相关信息</param>
        /// <exception cref="ArgumentNullException">当key或context为null时抛出</exception>
        public void NotifySingle(string key, NoticeContext context)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (context == null) throw new ArgumentNullException(nameof(context));

            lock (_lock)
            {
                if (_keyToObserverMap.TryGetValue(key, out var observer))
                {
                    observer.UpdateNotice(context);
                }
                // 如果找不到对应的观察者，这里可以选择抛出异常或者静默处理
                // 考虑到通知系统的特性，静默处理可能更合适
            }
        }
    }
}