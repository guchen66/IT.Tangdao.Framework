using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Alarms
{
    internal sealed class AlarmService : IAlarmService, IDisposable
    {
        private readonly AlarmPublisher _publisher = new AlarmPublisher();

        public IDisposable Subscribe(IObserver<AlarmMessage> observer) => _publisher.Subscribe(observer);

        public void Publish(string alarmName, bool isCritical = false)
            => _publisher.Publish(new AlarmMessage { Name = alarmName, IsCritical = isCritical });

        public void Dispose() => _publisher.Dispose();
    }
}