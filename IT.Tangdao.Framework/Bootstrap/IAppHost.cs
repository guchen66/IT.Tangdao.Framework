using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Events.Delegates;
using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.DaoTasks;

namespace IT.Tangdao.Framework.Bootstrap
{
    /// <summary>
    /// 提供机器宿主可回调钩子
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IAppHost<T>
    {
        /// <summary>
        /// 提供事件通道
        /// </summary>
        TangdaoPipe<T> Handler { get; }

        /// <summary>
        /// 提供异步任务流
        /// </summary>
        /// <param name="taskQueueManager"></param>
        /// <returns></returns>
        Task AsyncTaskHandler(ITaskQueueManager taskQueueManager);
    }

    public class TangdaoHost
    {
    }
}