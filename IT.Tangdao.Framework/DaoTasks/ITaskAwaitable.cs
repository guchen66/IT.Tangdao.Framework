using IT.Tangdao.Framework.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务可等待接口
    /// </summary>
    /// <remarks>
    /// 定义任务的基本属性和生命周期方法，由任务调度器使用
    /// </remarks>
    public interface ITaskAwaitable
    {
        /// <summary>
        /// 获取任务的当前状态
        /// </summary>
        TaskStatus Status { get; }

        /// <summary>
        /// 获取任务执行结果
        /// </summary>
        IResponseResult Result { get; }

        /// <summary>
        /// 任务完成事件
        /// </summary>
        event Action OnCompleted;

        /// <summary>
        /// 任务失败事件
        /// </summary>
        event Action<Exception> OnFaulted;

        /// <summary>
        /// 标记任务成功完成
        /// </summary>
        void Complete();

        /// <summary>
        /// 标记任务失败
        /// </summary>
        void Fault(Exception ex);

        /// <summary>
        /// 标记任务失败
        /// </summary>
        void Fault(IResponseResult result);
    }
}