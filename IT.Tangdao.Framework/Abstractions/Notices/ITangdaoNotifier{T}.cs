using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知接收器接口，用于接收发布者发送的通知
    /// </summary>
    /// <typeparam name="TNotification">通知类型</typeparam>
    public interface ITangdaoNotifier<TNotification> : IObserver<TNotification>, IDisposable
    {
    }
}