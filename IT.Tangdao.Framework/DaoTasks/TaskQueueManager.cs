using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Infrastructure;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.DaoTasks
{
    public class TaskQueueManager : ITaskQueueManager, IDisposable
    {
        private readonly ManualResetEventSlim _manual = new ManualResetEventSlim();
        private CancellationTokenSource _cts;
        private volatile bool _isPaused;
        private readonly object _lock = new object();

        // 任务队列，按优先级排序
        private readonly SortedDictionary<TaskPriority, Queue<TaskItem>> _taskQueue = new SortedDictionary<TaskPriority, Queue<TaskItem>>();

        // 任务字典，用于快速查找
        private readonly Dictionary<Guid, TaskItem> _taskDict = new Dictionary<Guid, TaskItem>();

        // 任务执行任务
        private Task _executionTask;

        public TaskQueueManager()
        {
            // 初始化不同优先级的队列
            foreach (TaskPriority priority in Enum.GetValues(typeof(TaskPriority)))
            {
                _taskQueue.Add(priority, new Queue<TaskItem>());
            }
        }

        public async Task StartAsync(IProgress<IAddTaskItem> progress)
        {
            lock (_lock)
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                _manual.Set();
                _isPaused = false;
            }

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    _cts.Token.ThrowIfCancellationRequested();

                    Random random = new Random();
                    int randomNumber = random.Next(0, 11);
                    bool result = randomNumber > 5;
                    progress.Report(new AddTaskItem()
                    {
                        NewItem = "未完成",
                    });

                    await Task.Delay(1000, _cts.Token);
                    if (_isPaused)
                    {
                        await Task.Run(() => { _manual.Wait(_cts.Token); });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("任务已取消");
            }
        }

        public void Pause()
        {
            lock (_lock)
            {
                _isPaused = true;
                _manual.Reset();
            }
        }

        public void Resume()
        {
            lock (_lock)
            {
                _isPaused = false;
                _manual.Set();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                _cts?.Cancel();
            }
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

            lock (_lock)
            {
                _taskQueue[priority].Enqueue(taskItem);
                _taskDict.Add(taskItem.Id, taskItem);

                // 如果执行任务未运行，启动执行任务
                if (_executionTask == null || _executionTask.IsCompleted)
                {
                    _executionTask = ExecuteTasksAsync();
                }
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
            lock (_lock)
            {
                if (_taskDict.TryGetValue(taskId, out var taskItem))
                {
                    // 只能取消未开始的任务
                    if (taskItem.Status == TaskStatus.Created)
                    {
                        // 从字典中移除
                        _taskDict.Remove(taskId);

                        // 从队列中移除（需要重新创建队列）
                        var newQueue = new Queue<TaskItem>(_taskQueue[taskItem.Priority].Where(t => t.Id != taskId));
                        _taskQueue[taskItem.Priority] = newQueue;

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
                return _taskDict.Values.ToList<ITaskItem>();
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
        /// 执行任务队列
        /// </summary>
        private async Task ExecuteTasksAsync()
        {
            while (true)
            {
                TaskItem taskItem = null;

                // 按优先级从高到低查找任务
                lock (_lock)
                {
                    foreach (var priority in Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>().Reverse())
                    {
                        if (_taskQueue[priority].Count > 0)
                        {
                            taskItem = _taskQueue[priority].Dequeue();
                            break;
                        }
                    }

                    // 如果没有任务，退出循环
                    if (taskItem == null)
                    {
                        break;
                    }

                    // 更新任务状态
                    taskItem.Status = TaskStatus.Running;
                    taskItem.StartTime = DateTime.Now;
                }

                try
                {
                    // 执行任务
                    var result = await taskItem.Action(_cts.Token);

                    // 更新任务状态
                    lock (_lock)
                    {
                        taskItem.Status = TaskStatus.RanToCompletion;
                        taskItem.Result = result;
                        taskItem.CompletedTime = DateTime.Now;
                    }
                }
                catch (OperationCanceledException)
                {
                    lock (_lock)
                    {
                        taskItem.Status = TaskStatus.Canceled;
                        taskItem.CompletedTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    lock (_lock)
                    {
                        taskItem.Status = TaskStatus.Faulted;
                        taskItem.Exception = ex;
                        taskItem.CompletedTime = DateTime.Now;
                    }
                }
                finally
                {
                    // 检查是否需要暂停
                    if (_isPaused)
                    {
                        await Task.Run(() => { _manual.Wait(_cts.Token); });
                    }
                }
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _manual?.Dispose();
        }
    }
}