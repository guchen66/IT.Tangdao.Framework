using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.EventArg;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 进度报告实现类
    /// </summary>
    /// <remarks>
    /// 实现了IProgressReport接口，用于报告任务执行进度
    /// </remarks>
    public class ProgressReport : IProgressReport
    {
        private long _total = 100;
        private long _completed = 0;
        private string _statusText = string.Empty;
        private bool _isCancelled = false;
        private readonly object _lock = new object();

        // 内部使用.NET内置的Progress<T>来处理进度报告
        private readonly Progress<TangdaoProgressChangedEventArgs> _internalProgress;

        /// <summary>
        /// 进度更新事件
        /// </summary>
        public event EventHandler<TangdaoProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// 取消事件
        /// </summary>
        public event EventHandler<CancellationEventArgs> Cancelled;

        /// <summary>
        /// 初始化ProgressReport类的新实例
        /// </summary>
        public ProgressReport() : this(null)
        {
        }

        /// <summary>
        /// 初始化ProgressReport类的新实例
        /// </summary>
        /// <param name="handler">进度更新处理程序</param>
        public ProgressReport(Action<TangdaoProgressChangedEventArgs> handler)
        {
            // 初始化内部Progress<T>，利用其内置的线程调度机制确保事件在正确的线程上触发
            _internalProgress = new Progress<TangdaoProgressChangedEventArgs>(args =>
            {
                // 首先调用外部处理程序（如果有）
                handler?.Invoke(args);
                // 然后触发公共事件
                ProgressChanged?.Invoke(this, args);
            });
        }

        /// <summary>
        /// 获取或设置任务总工作量
        /// </summary>
        public long Total
        {
            get => _total;
            set
            {
                lock (_lock)
                {
                    if (_total != value)
                    {
                        _total = value;
                        OnProgressChanged();
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置已完成工作量
        /// </summary>
        public long Completed
        {
            get => _completed;
            set
            {
                lock (_lock)
                {
                    if (_completed != value)
                    {
                        _completed = value;
                        OnProgressChanged();
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置当前状态文本
        /// </summary>
        public string StatusText
        {
            get => _statusText;
            set
            {
                lock (_lock)
                {
                    if (_statusText != value)
                    {
                        _statusText = value;
                        OnProgressChanged();
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前进度百分比
        /// </summary>
        public double ProgressPercentage
        {
            get
            {
                lock (_lock)
                {
                    if (_total <= 0)
                        return 0;

                    double percentage = (_completed * 100.0) / _total;
                    return Math.Min(100.0, Math.Max(0.0, percentage));
                }
            }
        }

        /// <summary>
        /// 获取或设置是否已取消
        /// </summary>
        public bool IsCancelled
        {
            get => _isCancelled;
            set
            {
                lock (_lock)
                {
                    if (_isCancelled != value)
                    {
                        _isCancelled = value;
                        if (_isCancelled)
                        {
                            OnCancelled();
                        }
                        OnProgressChanged();
                    }
                }
            }
        }

        /// <summary>
        /// 实现IProgress<T>.Report方法
        /// </summary>
        /// <param name="value">进度事件参数</param>
        void IProgress<TangdaoProgressChangedEventArgs>.Report(TangdaoProgressChangedEventArgs value)
        {
            // 使用内部Progress<T>来报告进度，通过接口调用Report方法以兼容所有.NET版本
            ((IProgress<TangdaoProgressChangedEventArgs>)_internalProgress).Report(value);
        }

        /// <summary>
        /// 报告进度更新
        /// </summary>
        /// <param name="completed">已完成工作量</param>
        /// <param name="total">总工作量</param>
        /// <param name="statusText">状态文本</param>
        public void Report(long completed, long total, string statusText = null)
        {
            lock (_lock)
            {
                bool changed = false;

                if (_completed != completed)
                {
                    _completed = completed;
                    changed = true;
                }

                if (_total != total)
                {
                    _total = total;
                    changed = true;
                }

                if (!string.Equals(_statusText, statusText))
                {
                    _statusText = statusText ?? string.Empty;
                    changed = true;
                }

                if (changed)
                {
                    OnProgressChanged();
                }
            }
        }

        /// <summary>
        /// 报告进度更新
        /// </summary>
        /// <param name="progressPercentage">进度百分比（0-100）</param>
        /// <param name="statusText">状态文本</param>
        public void Report(double progressPercentage, string statusText = null)
        {
            lock (_lock)
            {
                // 确保进度百分比在0-100之间
                progressPercentage = Math.Min(100.0, Math.Max(0.0, progressPercentage));

                // 根据百分比计算已完成工作量
                long completed = (long)(progressPercentage * _total / 100.0);

                bool changed = false;

                if (_completed != completed)
                {
                    _completed = completed;
                    changed = true;
                }

                if (!string.Equals(_statusText, statusText))
                {
                    _statusText = statusText ?? string.Empty;
                    changed = true;
                }

                if (changed)
                {
                    OnProgressChanged();
                }
            }
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        public void Cancel()
        {
            IsCancelled = true;
        }

        /// <summary>
        /// 检查是否已取消，如果已取消则抛出异常
        /// </summary>
        /// <exception cref="OperationCanceledException">如果已取消</exception>
        /// <remarks>
        /// 此方法用于在任务执行过程中检查取消状态
        /// 可以与CancellationToken结合使用，提供统一的取消检查机制
        /// </remarks>
        public void ThrowIfCancellationRequested()
        {
            if (IsCancelled)
            {
                throw new OperationCanceledException("任务已被取消");
            }
        }

        /// <summary>
        /// 检查是否已取消，如果已取消则抛出异常
        /// </summary>
        /// <param name="cancellationToken">可选的取消令牌，用于额外的取消检查</param>
        /// <exception cref="OperationCanceledException">如果已取消</exception>
        /// <remarks>
        /// 此重载方法允许同时检查进度报告的取消状态和CancellationToken的取消状态
        /// 提供了更灵活的取消检查机制
        /// </remarks>
        public void ThrowIfCancellationRequested(CancellationToken cancellationToken)
        {
            // 首先检查进度报告的取消状态
            ThrowIfCancellationRequested();
            // 然后检查取消令牌的取消状态
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 触发进度更新事件
        /// </summary>
        protected virtual void OnProgressChanged()
        {
            var args = new TangdaoProgressChangedEventArgs
            {
                ProgressPercentage = ProgressPercentage,
                StatusText = StatusText,
                Total = Total,
                Completed = Completed,
                IsCancelled = IsCancelled
            };

            // 使用内部Progress<T>来报告进度，这样可以确保在正确的线程上触发事件
            // 通过接口调用Report方法以兼容所有.NET版本
            ((IProgress<TangdaoProgressChangedEventArgs>)_internalProgress).Report(args);
        }

        /// <summary>
        /// 触发取消事件
        /// </summary>
        protected virtual void OnCancelled()
        {
            Cancelled?.Invoke(this, new CancellationEventArgs { IsCancelled = true });
        }

        /// <summary>
        /// 从System.Progress<T>转换为IProgressReport
        /// </summary>
        /// <param name="progress">System.Progress<T>实例</param>
        /// <returns>IProgressReport实例</returns>
        public static IProgressReport FromProgress(Progress<TangdaoProgressChangedEventArgs> progress)
        {
            // 通过接口调用Report方法以兼容所有.NET版本
            return new ProgressReport(args => ((IProgress<TangdaoProgressChangedEventArgs>)progress).Report(args));
        }

        /// <summary>
        /// 从IProgressReport转换为System.Progress<T>
        /// </summary>
        /// <param name="progressReport">IProgressReport实例</param>
        /// <returns>System.Progress<T>实例</returns>
        public static Progress<TangdaoProgressChangedEventArgs> ToProgress(IProgressReport progressReport)
        {
            var progress = new Progress<TangdaoProgressChangedEventArgs>();
            // 通过接口调用Report方法以兼容所有.NET版本
            progressReport.ProgressChanged += (sender, args) => ((IProgress<TangdaoProgressChangedEventArgs>)progress).Report(args);
            return progress;
        }

        /// <summary>
        /// 隐式转换为System.Progress<T>
        /// </summary>
        /// <param name="progressReport">IProgressReport实例</param>
        public static implicit operator Progress<TangdaoProgressChangedEventArgs>(ProgressReport progressReport)
        {
            return ToProgress(progressReport);
        }
    }
}