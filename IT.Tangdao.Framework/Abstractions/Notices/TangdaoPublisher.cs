using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 事件发布者
    /// </summary>
    public class TangdaoPublisher : ITangdaoPublisher
    {
        private readonly List<IObserver<MessageEventArgs>> _observers = new List<IObserver<MessageEventArgs>>();
        private readonly object _lock = new object();

        public IDisposable Subscribe(IObserver<MessageEventArgs> observer)
        {
            lock (_lock)
            {
                Console.WriteLine("TangdaoPublisher调用Subscribe");
                _observers.Add(observer);
                return new Unsubscriber(_observers, observer, _lock);
            }
        }

        public void Publish(MessageEventArgs message)
        {
            lock (_lock)
            {
                Console.WriteLine("TangdaoPublisher调用Publish");
                foreach (var obs in _observers.ToArray()) obs.OnNext(message);
            }
        }

        public void CompleteAll()
        {
            lock (_lock)
            {
                Console.WriteLine("TangdaoPublisher调用CompleteAll");
                foreach (var obs in _observers.ToArray()) obs.OnCompleted();
                _observers.Clear();
            }
        }

        public void Dispose() => CompleteAll();

        private sealed class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<MessageEventArgs>> _observers;
            private readonly IObserver<MessageEventArgs> _observer;
            private readonly object _lock;

            public Unsubscriber(List<IObserver<MessageEventArgs>> observers, IObserver<MessageEventArgs> observer, object @lock)
            {
                _observers = observers;
                _observer = observer;
                _lock = @lock;
            }

            public void Dispose()
            {
                lock (_lock)
                {
                    if (_observer != null && _observers.Contains(_observer))
                        _observers.Remove(_observer);
                }
            }
        }
    }
}