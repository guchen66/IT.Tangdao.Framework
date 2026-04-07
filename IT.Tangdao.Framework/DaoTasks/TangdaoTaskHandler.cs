using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    internal class TangdaoTaskHandler : IDisposable
    {
        private readonly BlockingCollection<TaskItem> _taskQueue = new BlockingCollection<TaskItem>();
        private readonly Task _processingTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private volatile bool _isPaused = false;
        private readonly ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);
        private bool _isDisposed = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TangdaoTaskHandler()
        {
            _processingTask = Task.Run(async () => await ProcessTaskQueueAsync());
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Dispose();
        }

        /// <summary>
        /// 将任务加入队列
        /// </summary>
        /// <param name="taskItem">任务项</param>
        public void EnqueueTask(TaskItem taskItem)
        {
            _taskQueue.Add(taskItem);
        }

        /// <summary>
        /// 处理任务队列
        /// </summary>
        private async Task ProcessTaskQueueAsync()
        {
            try
            {
                foreach (var taskItem in _taskQueue.GetConsumingEnumerable(_cts.Token))
                {
                    // 检查是否暂停
                    _pauseEvent.Wait(_cts.Token);

                    // 检查任务是否已取消
                    if (taskItem.Status == TaskStatus.Canceled)
                    {
                        continue;
                    }

                    await ProcessTaskAsync(taskItem);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，无需处理
            }
            catch (Exception ex)
            {
                // 避免任务处理线程崩溃
                Console.WriteLine($"任务处理线程异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理单个任务
        /// </summary>
        /// <param name="taskItem">任务项</param>
        private async Task ProcessTaskAsync(TaskItem taskItem)
        {
            // 更新任务状态
            taskItem.Status = TaskStatus.Running;
            taskItem.StartTime = DateTime.Now;

            try
            {
                // 执行任务
                var result = await taskItem.Action(_cts.Token);

                // 更新任务状态
                taskItem.Status = TaskStatus.RanToCompletion;
                taskItem.Result = result;
                taskItem.CompletedTime = DateTime.Now;
            }
            catch (OperationCanceledException)
            {
                // 更新任务状态
                taskItem.Status = TaskStatus.Canceled;
                taskItem.CompletedTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                // 更新任务状态
                taskItem.Status = TaskStatus.Faulted;
                taskItem.Exception = ex;
                taskItem.CompletedTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 暂停任务处理
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
            _pauseEvent.Reset();
        }

        /// <summary>
        /// 恢复任务处理
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
            _pauseEvent.Set();
        }

        /// <summary>
        /// 停止任务处理
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                // 标记队列完成，不再接受新的任务
                _taskQueue.CompleteAdding();
                // 取消所有任务
                _cts.Cancel();
                // 等待任务处理完成
                try
                {
                    _processingTask.Wait(1000); // 最多等待1秒
                }
                catch (Exception)
                {
                    // 忽略超时异常
                }
                // 清理资源
                _cts.Dispose();
                _taskQueue.Dispose();
                _pauseEvent.Dispose();
            }
        }
    }
}