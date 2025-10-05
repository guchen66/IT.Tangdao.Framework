using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Alarms
{
    public interface IAlarmService
    {
        IDisposable Subscribe(IObserver<AlarmMessage> observer);

        void Publish(string alarmName, bool isCritical = false);
    }
}