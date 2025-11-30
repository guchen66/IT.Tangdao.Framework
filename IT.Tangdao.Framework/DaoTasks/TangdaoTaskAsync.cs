using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 异步任务执行上下文
    /// </summary>
    /// <remarks>
    /// 用于异步任务的执行状态管理、生命周期跟踪和取消操作
    /// 支持通过CancellationToken取消正在执行的任务
    /// </remarks>
    public sealed class TangdaoTaskAsync : ITaskAwaitable, IDisposable
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

        #region ---- 取消令牌 ----

        /// <summary>
        /// 获取或设置任务的取消令牌
        /// </summary>
        /// <remarks>
        /// 由调度器注入，业务代码只读
        /// </remarks>
        public CancellationToken CancellationToken { get; internal set; }

        #endregion ---- 取消令牌 ----

        #region ---- 生命周期钩子（调度器调用） ----

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

        #endregion ---- 生命周期钩子（调度器调用） ----

        #region ---- 业务侧辅助方法 ----

        /// <summary>
        /// 如果已请求取消，则抛出OperationCanceledException
        /// </summary>
        /// <exception cref="OperationCanceledException">如果已请求取消</exception>
        public void ThrowIfCancellationRequested()
        {
            // 使用ProgressReport的重载方法，同时检查进度报告和取消令牌的取消状态
            _progressReport.ThrowIfCancellationRequested(CancellationToken);
        }

        /// <summary>
        /// 检查是否已请求取消
        /// </summary>
        /// <returns>如果已请求取消，则为true；否则为false</returns>
        public bool IsCancellationRequested => CancellationToken.IsCancellationRequested;

        /// <summary>
        /// 注册取消回调
        /// </summary>
        /// <param name="callback">取消时要执行的回调</param>
        /// <returns>一个可用于取消注册的CancellationTokenRegistration</returns>
        public CancellationTokenRegistration Register(Action callback)
            => CancellationToken.Register(callback);

        /// <summary>
        /// 注册取消回调
        /// </summary>
        /// <param name="callback">取消时要执行的回调</param>
        /// <param name="state">要传递给回调的状态</param>
        /// <returns>一个可用于取消注册的CancellationTokenRegistration</returns>
        public CancellationTokenRegistration Register(Action<object> callback, object state)
            => CancellationToken.Register(callback, state);

        #endregion ---- 业务侧辅助方法 ----

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