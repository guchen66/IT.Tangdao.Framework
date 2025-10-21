using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 毫秒级高精度 + CPU 负载自我节流的双模式计时器
    /// </summary>
    public sealed class CoroutineTimer : IDisposable
    {
        private readonly Stopwatch _watch = new Stopwatch();
        private readonly Func<Task> _tick;
        private readonly int _periodMs;
        private readonly bool _adaptive;
        private CancellationTokenSource _cts;
        private Task _runningTask;

        /// <param name="periodMs">周期(毫秒)</param>
        /// <param name="adaptive">是否根据 CPU % 自动降频</param>
        /// <param name="tick">异步回调，返回 Task</param>
        public CoroutineTimer(int periodMs, bool adaptive, Func<Task> tick)
        {
            if (periodMs <= 0) throw new ArgumentOutOfRangeException(nameof(periodMs));
            _periodMs = periodMs;
            _adaptive = adaptive;
            _tick = tick ?? throw new ArgumentNullException(nameof(tick));
        }

        public void Start()
        {
            Stop();
            _cts = new CancellationTokenSource();
            _runningTask = Loop(_cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
            _runningTask?.Wait();
            _cts?.Dispose();
            _cts = null;
        }

        private async Task Loop(CancellationToken token)
        {
            _watch.Restart();
            while (!token.IsCancellationRequested)
            {
                // 根据负载自适应跳过节拍
                if (_adaptive && CpuLoadTimer.ShouldSkip())
                {
                    await Task.Delay(16, token);
                    continue;
                }

                var next = _watch.ElapsedMilliseconds + _periodMs;
                await _tick();                                          // 用户异步回调
                var remain = (int)(next - _watch.ElapsedMilliseconds);
                if (remain > 0) await Task.Delay(remain, token);      // 漂移补偿
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}