using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务流门面类
    /// 采用门面模式，组合 ITaskQueueManager 和 ITaskController
    /// 为用户提供统一、简化的任务管理入口
    /// 无需用户手动注册IOC，即可快速使用任务系统
    /// </summary>
    public class TaskFlow : ITaskQueueManager, ITaskController, IDisposable
    {
        /// <summary>
        /// 任务队列管理器
        /// </summary>
        private readonly ITaskQueueManager _taskQueueManager;

        /// <summary>
        /// 任务控制器
        /// </summary>
        private readonly ITaskController _taskController;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 默认实例
        /// </summary>
        private static readonly Lazy<TaskFlow> _defaultInstance = new Lazy<TaskFlow>(() => new TaskFlow());

        /// <summary>
        /// 获取默认实例
        /// </summary>
        public static TaskFlow Default => _defaultInstance.Value;

        /// <summary>
        /// 构造函数
        /// 自动组合 TaskQueueManager 和 TaskController
        /// 为用户隐藏依赖组装的复杂性
        /// </summary>
        public TaskFlow()
        {
            _taskQueueManager = new TaskQueueManager();
            _taskController = new TaskController(_taskQueueManager);
        }

        /// <summary>
        /// 构造函数（用于依赖注入场景）
        /// 允许用户自定义注入 ITaskQueueManager 和 ITaskController
        /// </summary>
        /// <param name="taskQueueManager">任务队列管理器</param>
        /// <param name="taskController">任务控制器</param>
        public TaskFlow(ITaskQueueManager taskQueueManager, ITaskController taskController)
        {
            _taskQueueManager = taskQueueManager ?? throw new ArgumentNullException(nameof(taskQueueManager));
            _taskController = taskController ?? throw new ArgumentNullException(nameof(taskController));
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
            return _taskQueueManager.AddTask(name, action, priority);
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
            return _taskQueueManager.AddUITask(name, action, priority);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否取消成功</returns>
        public bool CancelTask(Guid taskId)
        {
            return _taskQueueManager.CancelTask(taskId);
        }

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns>任务列表</returns>
        public List<ITaskItem> GetAllTasks()
        {
            return _taskQueueManager.GetAllTasks();
        }

        /// <summary>
        /// 空的任务
        /// </summary>
        /// <returns></returns>
        public Task Empty()
        {
            return _taskQueueManager.Empty();
        }

        /// <summary>
        /// 获取任务数量
        /// </summary>
        /// <returns>任务数量</returns>
        public int GetTaskCount()
        {
            return _taskQueueManager.GetTaskCount();
        }

        /// <summary>
        /// 异步启动任务服务
        /// </summary>
        /// <param name="progress">用于报告任务进度的进度对象</param>
        /// <returns>表示异步操作的任务</returns>
        public Task StartAsync(IProgress<ITaskItem> progress)
        {
            return _taskController.StartAsync(progress);
        }

        /// <summary>
        /// 暂停任务服务
        /// </summary>
        public void Pause()
        {
            _taskController.Pause();
        }

        /// <summary>
        /// 恢复任务服务
        /// </summary>
        public void Resume()
        {
            _taskController.Resume();
        }

        /// <summary>
        /// 停止任务服务
        /// </summary>
        public void Stop()
        {
            _taskController.Stop();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                (_taskController as IDisposable)?.Dispose();
                _taskQueueManager.Dispose();
            }
        }
    }
}