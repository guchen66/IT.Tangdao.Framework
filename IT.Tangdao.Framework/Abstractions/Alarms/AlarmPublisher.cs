using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Alarms
{
    public sealed class AlarmPublisher : IObservable<AlarmMessage>, IDisposable
    {
        private readonly List<IObserver<AlarmMessage>> _observers = new List<IObserver<AlarmMessage>>();
        private readonly object _lock = new object();

        public IDisposable Subscribe(IObserver<AlarmMessage> observer)
        {
            lock (_lock)
            {
                _observers.Add(observer);
                return new Unsubscriber(_observers, observer, _lock);
            }
        }

        public void Publish(AlarmMessage message)
        {
            lock (_lock)
            {
                foreach (var obs in _observers.ToArray()) obs.OnNext(message);
            }
        }

        public void CompleteAll()
        {
            lock (_lock)
            {
                foreach (var obs in _observers.ToArray()) obs.OnCompleted();
                _observers.Clear();
            }
        }

        public void Dispose() => CompleteAll();

        private sealed class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<AlarmMessage>> _observers;
            private readonly IObserver<AlarmMessage> _observer;
            private readonly object _lock;

            public Unsubscriber(List<IObserver<AlarmMessage>> observers,
                                IObserver<AlarmMessage> observer,
                                object @lock)
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