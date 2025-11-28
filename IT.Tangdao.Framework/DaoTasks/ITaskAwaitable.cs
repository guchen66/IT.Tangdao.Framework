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
    /// 定义了任务生命周期的钩子方法，由任务调度器调用
    /// </remarks>
    internal interface ITaskAwaitable
    {
        /// <summary>
        /// 任务完成时调用
        /// </summary>
        void OnCompleted();

        /// <summary>
        /// 任务执行出错时调用
        /// </summary>
        /// <param name="ex">任务执行过程中发生的异常</param>
        void OnFaulted(Exception ex);
    }
}