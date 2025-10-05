using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Alarms
{
    public interface IAlarmNotifier : IObserver<AlarmMessage>, IDisposable
    {
        string Name { get; }
    }
}