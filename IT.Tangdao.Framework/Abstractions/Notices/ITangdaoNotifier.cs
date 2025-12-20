using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知接收器接口，用于接收发布者发送的通用通知
    /// </summary>
    public interface ITangdaoNotifier : ITangdaoNotifier<object>, IDisposable
    {
    }
}