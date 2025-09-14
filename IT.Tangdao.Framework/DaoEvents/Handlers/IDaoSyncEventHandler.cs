using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEvents.Handlers
{
    /// <summary>
    /// 同步事件处理程序接口
    /// </summary>
    public interface IDaoSyncEventHandler<in TEvent> where TEvent : DaoEventBase
    {
        void Handle(TEvent @event);
    }
}