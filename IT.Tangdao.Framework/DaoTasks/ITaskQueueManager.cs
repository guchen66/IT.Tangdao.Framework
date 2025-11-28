using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 增强的任务服务接口
    /// </summary>
    public interface ITaskQueueManager : ITaskQueue
    {
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="action">任务操作</param>
        /// <param name="priority">任务优先级</param>
        /// <returns>任务ID</returns>
        Guid AddTask(string name, Func<CancellationToken, Task<object>> action, TaskPriority priority = TaskPriority.Normal);

        /// <summary>
        /// 添加UI任务
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="action">任务操作</param>
        /// <param name="priority">任务优先级</param>
        /// <returns>任务ID</returns>
        Guid AddUITask(string name, Func<CancellationToken, Task<object>> action, TaskPriority priority = TaskPriority.Normal);

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否取消成功</returns>
        bool CancelTask(Guid taskId);

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns>任务列表</returns>
        List<ITaskItem> GetAllTasks();

        /// <summary>
        /// 获取任务数量
        /// </summary>
        /// <returns>任务数量</returns>
        int GetTaskCount();
    }
}