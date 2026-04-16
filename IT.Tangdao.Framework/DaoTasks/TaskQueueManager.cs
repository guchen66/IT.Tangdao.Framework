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
    /// <summary>
    /// 任务队列管理器
    /// 负责任务的添加、取消、查询等队列管理操作
    /// 遵循单一职责原则，仅关注任务队列的管理
    /// </summary>
    public class TaskQueueManager : ITaskQueueManager
    {
        /// <summary>
        /// 任务字典，用于快速查找任务
        /// </summary>
        private readonly Dictionary<Guid, TaskItem> _taskDict = new Dictionary<Guid, TaskItem>();

        /// <summary>
        /// 任务队列，按优先级排序
        /// </summary>
        private readonly List<TaskItem> _taskQueue = new List<TaskItem>();

        /// <summary>
        /// 线程安全锁
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool _isDisposed = false;

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

            lock (_lock)
            {
                _taskDict.Add(taskItem.Id, taskItem);

                int insertIndex = 0;
                foreach (var item in _taskQueue)
                {
                    if (item.Priority < taskItem.Priority)
                        break;
                    insertIndex++;
                }
                _taskQueue.Insert(insertIndex, taskItem);
            }

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
            lock (_lock)
            {
                if (_taskDict.TryGetValue(taskId, out var taskItem))
                {
                    if (taskItem.Status == TaskStatus.Created)
                    {
                        _taskDict.Remove(taskId);
                        _taskQueue.RemoveAll(t => t.Id == taskId);
                        taskItem.Status = TaskStatus.Canceled;
                        taskItem.CompletedTime = DateTime.Now;
                        taskItem.TCS.TrySetResult(taskItem);
                        return true;
                    }
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
            lock (_lock)
            {
                return _taskDict.Values.Cast<ITaskItem>().ToList();
            }
        }

        /// <summary>
        /// 获取任务数量
        /// </summary>
        /// <returns>任务数量</returns>
        public int GetTaskCount()
        {
            lock (_lock)
            {
                return _taskDict.Count;
            }
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
        /// 出队一个任务（内部方法）
        /// </summary>
        /// <returns>任务项，如果队列为空则返回null</returns>
        internal TaskItem Dequeue()
        {
            lock (_lock)
            {
                if (_taskQueue.Count == 0)
                    return null;

                var taskItem = _taskQueue[0];
                _taskQueue.RemoveAt(0);
                return taskItem;
            }
        }

        /// <summary>
        /// 检查队列是否为空（内部方法）
        /// </summary>
        /// <returns>是否为空</returns>
        internal bool IsEmpty()
        {
            lock (_lock)
            {
                return _taskQueue.Count == 0;
            }
        }

        /// <summary>
        /// 根据ID获取任务（内部方法）
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>任务项</returns>
        internal TaskItem GetTask(Guid taskId)
        {
            lock (_lock)
            {
                _taskDict.TryGetValue(taskId, out var taskItem);
                return taskItem;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                lock (_lock)
                {
                    foreach (var task in _taskDict.Values)
                    {
                        if (task.Status == TaskStatus.Created)
                        {
                            task.Status = TaskStatus.Canceled;
                            task.CompletedTime = DateTime.Now;
                        }
                    }
                    _taskDict.Clear();
                    _taskQueue.Clear();
                }
            }
        }
    }
}