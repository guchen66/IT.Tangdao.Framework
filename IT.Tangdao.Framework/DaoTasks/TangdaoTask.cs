using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 同步任务执行上下文
    /// </summary>
    /// <remarks>
    /// 用于同步任务的执行状态管理和生命周期跟踪
    /// </remarks>
    public sealed class TangdaoTask : ITaskAwaitable, IDisposable
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();
        private bool _disposed;                       // 防止重复 Dispose
        private readonly object _lock = new object(); // 线程安全锁
        private readonly ProgressReport _progressReport = new ProgressReport();

        #region ---- 对外只读状态 ----

        /// <summary>
        /// 获取任务已执行的时间跨度
        /// </summary>
        public TimeSpan Elapsed => _sw.Elapsed;

        /// <summary>
        /// 获取任务的当前状态
        /// </summary>
        public TaskStatus Status { get; private set; } = TaskStatus.Running;

        /// <summary>
        /// 获取任务执行过程中发生的异常
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// 获取任务是否成功完成
        /// </summary>
        public bool IsCompletedSuccessfully => Status == TaskStatus.RanToCompletion;

        /// <summary>
        /// 获取进度报告对象
        /// </summary>
        public IProgressReport ProgressReport => _progressReport;

        #endregion ---- 对外只读状态 ----

        #region ---- 生命周期钩子（调度器会调用） ----

        /// <summary>
        /// 任务完成时调用的钩子方法
        /// </summary>
        public void OnCompleted()
        {
            lock (_lock)
            {
                if (_disposed) return;
                Status = TaskStatus.RanToCompletion;
            }
        }

        /// <summary>
        /// 任务执行出错时调用的钩子方法
        /// </summary>
        /// <param name="ex">任务执行过程中发生的异常</param>
        public void OnFaulted(Exception ex)
        {
            lock (_lock)
            {
                if (_disposed) return;
                Status = TaskStatus.Faulted;
                Error = ex;
            }
        }

        #endregion ---- 生命周期钩子（调度器会调用） ----

        #region ---- IDisposable ----

        /// <summary>
        /// 释放任务资源
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed) return;
                _disposed = true;
                _sw.Stop();
                Status = Status == TaskStatus.Running
                            ? TaskStatus.Canceled
                            : Status;
            }
        }

        #endregion ---- IDisposable ----
    }
}