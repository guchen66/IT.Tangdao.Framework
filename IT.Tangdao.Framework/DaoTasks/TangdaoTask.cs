using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务包装器，提供状态追踪、耗时统计、进度报告和取消支持
    /// </summary>
    public sealed class TangdaoTask : IDisposable
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();
        private readonly object _lock = new object();
        private ProgressReport _progressReport;

        private TaskStatus _status = TaskStatus.Running;
        private bool _disposed;

        /// <summary>
        /// 获取任务已执行的时间跨度
        /// </summary>
        public TimeSpan Elapsed => _sw.Elapsed;

        /// <summary>
        /// 获取任务的当前状态
        /// </summary>
        public TaskStatus Status => _status;

        /// <summary>
        /// 获取进度报告对象
        /// </summary>
        /// <summary>
        /// 获取进度报告对象（懒加载）
        /// </summary>
        public IProgressReport ProgressReport
        {
            get
            {
                if (_progressReport == null)
                    _progressReport = new ProgressReport();
                return _progressReport;
            }
        }

        /// <summary>
        /// 取消令牌
        /// </summary>
        public CancellationToken CancellationToken { get; internal set; }

        /// <summary>
        /// 如果已请求取消，则抛出OperationCanceledException
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            _progressReport.ThrowIfCancellationRequested(CancellationToken);
        }

        /// <summary>
        /// 检查是否已请求取消
        /// </summary>
        public bool IsCancellationRequested => CancellationToken.IsCancellationRequested || _progressReport.IsCancelled;

        /// <summary>
        /// 注册取消回调
        /// </summary>
        public CancellationTokenRegistration Register(Action callback)
            => CancellationToken.Register(callback);

        /// <summary>
        /// 注册取消回调（带状态）
        /// </summary>
        public CancellationTokenRegistration Register(Action<object> callback, object state)
            => CancellationToken.Register(callback, state);

        /// <summary>
        /// 标记任务成功完成
        /// </summary>
        internal TangdaoTask SetResult()
        {
            lock (_lock)
            {
                if (_disposed || _status != TaskStatus.Running) return this;

                _status = TaskStatus.RanToCompletion;
                _progressReport?.Report(100, "任务执行成功");
                return this;
            }
        }

        /// <summary>
        /// 标记任务失败
        /// </summary>
        internal TangdaoTask SetException(Exception ex)
        {
            lock (_lock)
            {
                if (_disposed || _status != TaskStatus.Running) return this;

                _status = TaskStatus.Faulted;

                _progressReport?.Report(0, $"任务执行失败：{ex?.Message}");
                return this;
            }
        }

        /// <summary>
        /// 标记任务被取消
        /// </summary>
        internal void SetCanceled()
        {
            lock (_lock)
            {
                if (_disposed || _status != TaskStatus.Running) return;

                _status = TaskStatus.Canceled;
                _progressReport?.Report(0, "任务被取消");
            }
        }

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
                // 不修改 _status，职责分离
            }
        }
    }
}