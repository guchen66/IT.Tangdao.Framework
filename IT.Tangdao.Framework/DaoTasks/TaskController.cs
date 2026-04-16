using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务控制器
    /// 通过构造器注入 ITaskQueueManager，负责任务的启动、暂停、恢复和停止
    /// 支持多种线程调度模式：UI线程、后台线程、UI空闲时执行等
    /// 遵循组合优于继承原则，通过依赖注入解耦
    /// </summary>
    public class TaskController : ITaskController, IDisposable
    {
        /// <summary>
        /// 任务队列管理器
        /// 通过构造器注入，实现依赖倒置
        /// </summary>
        private readonly ITaskQueueManager _taskQueueManager;

        /// <summary>
        /// 取消令牌源
        /// </summary>
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        /// 暂停事件
        /// </summary>
        private readonly ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);

        /// <summary>
        /// 处理任务的任务
        /// </summary>
        private Task _processingTask;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool _isRunning = false;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 进度报告器
        /// </summary>
        private IProgress<ITaskItem> _progress;

        /// <summary>
        /// 默认线程类型
        /// </summary>
        private readonly TaskThreadType _defaultThreadType;

        /// <summary>
        /// 获取或设置当前的调度器
        /// </summary>
        private static Dispatcher Dispatcher =>
            Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        /// <summary>
        /// 构造函数
        /// 通过构造器注入任务队列管理器，实现依赖注入
        /// </summary>
        /// <param name="taskQueueManager">任务队列管理器</param>
        public TaskController(ITaskQueueManager taskQueueManager)
            : this(taskQueueManager, TaskThreadType.Auto)
        {
        }

        /// <summary>
        /// 构造函数
        /// 通过构造器注入任务队列管理器，并指定默认线程类型
        /// </summary>
        /// <param name="taskQueueManager">任务队列管理器</param>
        /// <param name="defaultThreadType">默认线程类型</param>
        public TaskController(ITaskQueueManager taskQueueManager, TaskThreadType defaultThreadType)
        {
            _taskQueueManager = taskQueueManager ?? throw new ArgumentNullException(nameof(taskQueueManager));
            _defaultThreadType = defaultThreadType;
        }

        /// <summary>
        /// 异步启动任务服务
        /// </summary>
        /// <param name="progress">用于报告任务进度的进度对象</param>
        /// <returns>表示异步操作的任务</returns>
        public Task StartAsync(IProgress<ITaskItem> progress)
        {
            lock (_cts)
            {
                if (_isRunning)
                    return Task.CompletedTask;

                _isRunning = true;
                _progress = progress;
                _processingTask = ProcessTaskQueueAsync();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 暂停任务服务
        /// </summary>
        public void Pause()
        {
            _pauseEvent.Reset();
        }

        /// <summary>
        /// 恢复任务服务
        /// </summary>
        public void Resume()
        {
            _pauseEvent.Set();
        }

        /// <summary>
        /// 停止任务服务
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
            _pauseEvent.Set();
        }

        /// <summary>
        /// 处理任务队列
        /// </summary>
        private async Task ProcessTaskQueueAsync()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    _pauseEvent.Wait(_cts.Token);

                    var taskItem = (_taskQueueManager as TaskQueueManager)?.Dequeue();

                    if (taskItem != null)
                    {
                        var threadType = taskItem.ThreadType != TaskThreadType.Auto
                            ? taskItem.ThreadType
                            : _defaultThreadType;
                        await ProcessTaskAsync(taskItem, threadType);
                    }
                    else
                    {
                        await Task.Delay(100, _cts.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"任务处理线程异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理单个任务
        /// </summary>
        /// <param name="taskItem">任务项</param>
        /// <param name="threadType">线程类型</param>
        private async Task ProcessTaskAsync(TaskItem taskItem, TaskThreadType threadType)
        {
            taskItem.Status = TaskStatus.Running;
            taskItem.StartTime = DateTime.Now;
            _progress?.Report(taskItem);

            try
            {
                object result = await ExecuteWithThreadType(() => taskItem.Action(_cts.Token), threadType);

                taskItem.Status = TaskStatus.RanToCompletion;
                taskItem.Result = ResponseResult<ITaskItem>.Success(taskItem, "任务完成");
                taskItem.CompletedTime = DateTime.Now;
                taskItem.TCS.TrySetResult(taskItem);
            }
            catch (OperationCanceledException)
            {
                taskItem.Status = TaskStatus.Canceled;
                taskItem.CompletedTime = DateTime.Now;
                taskItem.TCS.TrySetResult(taskItem);
            }
            catch (Exception ex)
            {
                taskItem.Status = TaskStatus.Faulted;
                taskItem.Exception = ex;
                taskItem.Result = ResponseResult<ITaskItem>.Failure(ex.Message, ex);
                taskItem.CompletedTime = DateTime.Now;
                taskItem.TCS.TrySetException(ex);
            }

            _progress?.Report(taskItem);
        }

        /// <summary>
        /// 根据线程类型执行异步操作
        /// </summary>
        /// <param name="func">要执行的异步函数</param>
        /// <param name="threadType">线程类型</param>
        /// <returns>任务执行结果</returns>
        private async Task<object> ExecuteWithThreadType(Func<Task<object>> func, TaskThreadType threadType)
        {
            switch (threadType)
            {
                case TaskThreadType.UI:
                    return await ExecuteOnUIThreadAsync(func);

                case TaskThreadType.Background:
                    return await Task.Run(func);

                case TaskThreadType.UIIdle:
                    return await ExecuteOnUIThreadAsync(func);

                case TaskThreadType.Auto:
                default:
                    if (Dispatcher.CheckAccess())
                    {
                        return await func();
                    }
                    else
                    {
                        return await ExecuteOnUIThreadAsync(func);
                    }
            }
        }

        /// <summary>
        /// 在UI线程上执行异步操作
        /// </summary>
        /// <param name="func">要执行的异步函数</param>
        /// <returns>任务执行结果</returns>
        private Task<TResult> ExecuteOnUIThreadAsync<TResult>(Func<Task<TResult>> func)
        {
            return Dispatcher.CheckAccess()
                   ? func()
                   : Dispatcher.InvokeAsync(func, DispatcherPriority.Normal).Task.Unwrap();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Stop();

                if (_processingTask != null)
                {
                    try
                    {
                        _processingTask.Wait(1000);
                    }
                    catch (Exception)
                    {
                    }
                }

                _cts.Dispose();
                _pauseEvent.Dispose();
                _taskQueueManager.Dispose();
            }
        }
    }
}