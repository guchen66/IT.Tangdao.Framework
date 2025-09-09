using IT.Tangdao.Framework.DaoInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    public sealed class TangdaoTaskAsync : IMarkable, IDisposable
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();
        private bool _disposed;
        private readonly object _lock = new object();

        // 1. 对外只读状态
        public TimeSpan Elapsed => _sw.Elapsed;

        public string Duration => _sw.Elapsed.ToString(@"hh\\:mm\\:ss\\.fff");
        public TaskStatus Status { get; private set; } = TaskStatus.Running;
        public Exception Error { get; private set; }
        public bool IsCompletedSuccessfully => Status == TaskStatus.RanToCompletion;

        // 2. 取消令牌源（由调度器注入，业务代码只读）
        public CancellationToken CancellationToken { get; internal set; }

        // 3. 生命周期钩子（调度器调用）
        public void MarkCompleted()
        {
            lock (_lock)
            {
                if (_disposed) return;
                Status = TaskStatus.RanToCompletion;
            }
        }

        public void MarkFaulted(Exception ex)
        {
            lock (_lock)
            {
                if (_disposed) return;
                Status = TaskStatus.Faulted;
                Error = ex;
            }
        }

        // 4. 业务侧 helpers
        public void ThrowIfCancellationRequested()
            => CancellationToken.ThrowIfCancellationRequested();

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
    }
}