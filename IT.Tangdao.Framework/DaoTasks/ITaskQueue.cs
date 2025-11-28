using IT.Tangdao.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务服务接口
    /// </summary>
    /// <remarks>
    /// 定义了任务服务的基本操作，包括启动、暂停、恢复和停止任务
    /// </remarks>
    public interface ITaskQueue
    {
        /// <summary>
        /// 异步启动任务服务
        /// </summary>
        /// <param name="progress">用于报告任务进度的进度对象</param>
        /// <returns>表示异步操作的任务</returns>
        Task StartAsync(IProgress<IAddTaskItem> progress);

        /// <summary>
        /// 暂停任务服务
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复任务服务
        /// </summary>
        void Resume();

        /// <summary>
        /// 停止任务服务
        /// </summary>
        void Stop();
    }
}