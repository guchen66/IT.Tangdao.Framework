using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Infrastructure;
using IT.Tangdao.Framework.Enums;
using System.Collections.Concurrent;

namespace IT.Tangdao.Framework.DaoTasks
{
    public class TaskQueueManager : ITaskQueueManager
    {
        private static readonly ConcurrentDictionary<Guid, TaskItem> _taskDict = new ConcurrentDictionary<Guid, TaskItem>();
        private static readonly TangdaoTaskHandler _taskHandler = new TangdaoTaskHandler();

        /// <summary>
        /// 暂停任务处理
        /// </summary>
        public void Pause()
        {
            _taskHandler.Pause();
        }

        /// <summary>
        /// 恢复任务处理
        /// </summary>
        public void Resume()
        {
            _taskHandler.Resume();
        }

        /// <summary>
        /// 停止任务处理
        /// </summary>
        public void Stop()
        {
            _taskHandler.Stop();
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="action">任务操作</param>
        /// <param name="priority">任务优先级</param>
        /// <returns>任务ID</returns>
        public Guid AddTask(string name, Func<CancellationToken, Task<object>> action, TaskPriority priority = TaskPriority.Normal)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var taskItem = new TaskItem(name, priority, action);
            _taskDict.TryAdd(taskItem.Id, taskItem);
            _taskHandler.EnqueueTask(taskItem);

            return taskItem.Id;
        }

        /// <summary>
        /// 添加UI任务
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="action">任务操作</param>
        /// <param name="priority">任务优先级</param>
        /// <returns>任务ID</returns>
        public Guid AddUITask(string name, Func<CancellationToken, Task<object>> action, TaskPriority priority = TaskPriority.Normal)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // UI任务包装，确保在UI线程执行
            Func<CancellationToken, Task<object>> uiAction = async (token) =>
            {
                return await TangdaoTaskScheduler.ExecuteAsync(async (task) =>
                {
                    return await action(token);
                });
            };

            return AddTask(name, uiAction, priority);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否取消成功</returns>
        public bool CancelTask(Guid taskId)
        {
            if (_taskDict.TryGetValue(taskId, out var taskItem))
            {
                // 只能取消未开始的任务
                if (taskItem.Status == TaskStatus.Created)
                {
                    // 从字典中移除
                    _taskDict.TryRemove(taskId, out _);
                    // 标记为取消状态
                    taskItem.Status = TaskStatus.Canceled;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns>任务列表</returns>
        public List<ITaskItem> GetAllTasks()
        {
            return _taskDict.Values.ToList<ITaskItem>();
        }

        /// <summary>
        /// 获取任务数量
        /// </summary>
        /// <returns>任务数量</returns>
        public int GetTaskCount()
        {
            return _taskDict.Count;
        }

        /// <summary>
        /// 空的任务
        /// </summary>
        /// <returns></returns>
        public Task Empty()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _taskHandler.Dispose();
        }
    }
}