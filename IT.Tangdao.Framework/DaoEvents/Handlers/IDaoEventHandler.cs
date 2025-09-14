using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEvents.Handlers
{
    /// <summary>
    /// 异步事件处理程序接口
    /// </summary>
    public interface IDaoEventHandler<in TEvent> where TEvent : DaoEventBase
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}