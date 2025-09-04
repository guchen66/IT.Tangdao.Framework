using IT.Tangdao.Framework.DaoInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public sealed class TangdaoTask : IMarkable, IDisposable
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();
        private bool _disposed;                       // 防止重复 Dispose
        private readonly object _lock = new object(); // 线程安全

        #region ---- 对外只读状态 ----

        public TimeSpan Elapsed => _sw.Elapsed;
        public string Duration => _sw.Elapsed.ToString(@"hh\:mm\:ss\.fff");
        public TaskStatus Status { get; private set; } = TaskStatus.Running;
        public Exception Error { get; private set; }
        public bool IsCompletedSuccessfully => Status == TaskStatus.RanToCompletion;

        #endregion ---- 对外只读状态 ----

        #region ---- 生命周期钩子（调度器会调用） ----

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

        #endregion ---- 生命周期钩子（调度器会调用） ----

        #region ---- IDisposable ----

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