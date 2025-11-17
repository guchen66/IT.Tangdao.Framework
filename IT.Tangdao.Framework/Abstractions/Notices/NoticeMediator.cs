using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知指令的中介者
    /// </summary>
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