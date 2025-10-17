using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 创建一个异步等待
    /// </summary>
    public static class TimeAwaitExtension
    {
        /// <summary>
        /// 让 TimeSpan 可以被 await，等同于 Task.Delay。
        /// 用途：在 UI 线程里“非阻塞”等待一段时间再继续。
        /// </summary>
        public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
            => Task.Delay(timeSpan).GetAwaiter();

        /// <summary>
        /// 同时等待时间和取消令牌，任意一个满足即结束（或抛取消）。
        /// 用途：给动画/轮询加“随时可停”能力。
        /// </summary>
        public static TaskAwaiter GetAwaiter(this (TimeSpan delay, CancellationToken token) tuple)
            => Task.Delay(tuple.delay, tuple.token).GetAwaiter();

        /// <summary>
        /// 等待 CountdownEvent 计数变为 0 才继续。
        /// 用途：并行任务都完成后才刷新 UI。
        /// </summary>
        public static TaskAwaiter GetAwaiter(this CountdownEvent cd)
            => Task.Run(() => cd.Wait()).GetAwaiter();

        public static Task WaitAsync(this CountdownEvent cd, CancellationToken token = default)
        {
            return Task.Run(() => cd.Wait(token));
        }

        /// <summary>
        /// 等待 ManualResetEventSlim 被 Set 才继续。
        /// 用途：跨线程/跨页面通知一次即触发。
        /// </summary>
        public static TaskAwaiter GetAwaiter(this ManualResetEventSlim mres)
            => Task.Run(() => mres.Wait()).GetAwaiter();

        /// <summary>
        /// 等待 CancellationToken 被取消；取消时抛出 OperationCanceledException。
        /// 用途：随时中断后台工作，UI 只关心“取消信号”。
        /// </summary>
        public static TaskAwaiter<bool> GetAwaiter(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
            return tcs.Task.GetAwaiter();
        }
    }
}