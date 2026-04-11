using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.Abstractions.Messaging
{
    /// <summary>
    /// 通知接收器实现，用于接收通用通知
    /// </summary>
    public class TangdaoNotifier : TangdaoNotifier<object>, ITangdaoNotifier
    {
    }
}