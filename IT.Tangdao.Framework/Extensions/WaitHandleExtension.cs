using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class WaitHandleExtension
    {
        /// <summary>
        /// 让任意 WaitHandle 支持 await（真正非阻塞）
        /// </summary>
        public static Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout = Timeout.Infinite, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (handle == null) throw new ArgumentNullException(nameof(handle));

            var tcs = new TaskCompletionSource<bool>();
            RegisteredWaitHandle registeredWait = null;

            // 回调：事件触发或超时
            WaitOrTimerCallback callback = (_, timedOut) =>
            {
                registeredWait?.Unregister(null);   // 立即清理
                if (cancellationToken.IsCancellationRequested)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(!timedOut);    // true=触发，false=超时
            };

            // 注册到线程池
            registeredWait = ThreadPool.RegisterWaitForSingleObject(
                handle,
                callback,
                null,
                millisecondsTimeout,
                executeOnlyOnce: true);

            // 如果外部取消，也主动 SetCanceled 并清理
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    registeredWait.Unregister(null);
                    tcs.TrySetCanceled();
                }, useSynchronizationContext: false);
            }

            return tcs.Task;
        }
    }
}