using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public interface INoticeFactory
    {
        INoticeObserver CreateObserver(NoticeContext naviceContext);
    }

    /// <summary>
    /// 通知作用域上下文，不可变
    /// </summary>
    public sealed class NoticeContext
    {
        public string Key { get; }
        public object Parameter { get; }

        public NoticeContext(string key, object parameter = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Parameter = parameter;
        }

        // 便于字典比较
        public override bool Equals(object obj)
            => obj is NoticeContext other && Key == other.Key;

        public override int GetHashCode() => Key.GetHashCode();
    }

    public sealed class DefaultNoticeFactory : INoticeFactory
    {
        // key → 类型的内部映射表
        private static readonly Dictionary<string, Type> _typeMap =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "Badge", typeof(BadgeObserver) },
                { "Alert", typeof(AlertObserver) }
                // 用户如果写了自己的 XXXObserver，在这里注册一行即可
            };

        public INoticeObserver CreateObserver(NoticeContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var key = context.Key;
            if (!_typeMap.TryGetValue(key, out var type))
                throw new NotSupportedException(
                    $"No observer registered for key '{key}'. " +
                    $"Call DefaultNoticeFactory.Register<{key}>() at module initializer.");

            // 零参数构造
            return (INoticeObserver)Activator.CreateInstance(type);
        }

        // 允许外部代码（通常是他们自己的模块初始化器）注入新类型
        public static void Register<T>(string key) where T : INoticeObserver
        {
            _typeMap[key] = typeof(T);
        }
    }

    public class NoticeMediator
    {
        private static readonly Lazy<NoticeMediator> _instance = new Lazy<NoticeMediator>(() => new NoticeMediator());

        public static NoticeMediator Instance => _instance.Value;

        private readonly List<INoticeObserver> _observers = new List<INoticeObserver>();
        private readonly object _lock = new object();

        public event Action<NoticeState> StateChanged;

        private NoticeMediator()
        { }

        public void Register(INoticeObserver observer)
        {
            lock (_lock)
            {
                if (!_observers.Contains(observer))
                {
                    _observers.Add(observer);
                }
            }
        }

        public void Unregister(INoticeObserver observer)
        {
            lock (_lock)
            {
                _observers.Remove(observer);
            }
        }

        public void NotifyStateChange(NoticeState state)
        {
            List<INoticeObserver> observersCopy;
            lock (_lock)
            {
                observersCopy = new List<INoticeObserver>(_observers);
            }

            foreach (var observer in observersCopy)
            {
                // 只通知关心此类型的观察者
                if (observer.Context == state.Context)
                {
                    observer.UpdateState(state);
                }
            }

            StateChanged?.Invoke(state); // 让 UI 层也能监听
        }

        public void NotifyAll(bool newState)
        {
            List<INoticeObserver> observersCopy;
            lock (_lock)
            {
                observersCopy = new List<INoticeObserver>(_observers);
            }

            foreach (var observer in observersCopy)
            {
                var state = new NoticeState
                {
                    IsActive = newState,
                    Context = observer.Context,
                    UpdateTime = DateTime.Now
                };
                observer.UpdateState(state);
            }
        }
    }
}