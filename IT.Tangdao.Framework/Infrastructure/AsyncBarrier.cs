using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 原生 Barrier 的 awaitable 版本：阶段推进不阻塞线程
    /// </summary>
    public sealed class AsyncBarrier : IDisposable
    {
        private readonly int _participantCount;
        private readonly Action<int> _phaseAction;          // 可选阶段结束回调
        private int _remaining;                             // 当前阶段剩余信号
        private int _phase;                                 // 阶段序号
        private TaskCompletionSource<int> _tcs;             // 阶段完成源
        private readonly object _lock = new object();

        public AsyncBarrier(int participantCount,
                            Action<int> phaseAction = null)
        {
            if (participantCount <= 0) throw new ArgumentOutOfRangeException(nameof(participantCount));
            _participantCount = participantCount;
            _phaseAction = phaseAction;
            _remaining = participantCount;
            _tcs = new TaskCompletionSource<int>();
        }

        /// <summary>
        /// 等待当前阶段所有参与者到达，然后自动进入下一阶段
        /// </summary>
        public Task<int> SignalAndWaitAsync(CancellationToken token = default(CancellationToken))
        {
            Task<int> waitTask;
            lock (_lock)
            {
                if (_remaining == 0) throw new InvalidOperationException("All participants have already signaled.");
                _remaining--;
                if (_remaining > 0)
                {
                    // 还有人在路上，返回当前 tcs.Task
                    waitTask = _tcs.Task;
                }
                else
                {
                    // 我是最后一个，完成当前阶段
                    _phaseAction?.Invoke(_phase);
                    var oldTcs = _tcs;
                    // 预先为下一阶段新建 tcs，避免竞态
                    _tcs = new TaskCompletionSource<int>();
                    int completedPhase = _phase;
                    // 推进阶段
                    _phase++;
                    _remaining = _participantCount;
                    // 释放所有等待者
                    oldTcs.SetResult(completedPhase);
                    waitTask = Task.FromResult(completedPhase);
                }
            }
            return token.IsCancellationRequested
                ? Task.FromCanceled<int>(token)
                : waitTask;
        }

        /// <summary>当前阶段编号</summary>
        public int CurrentPhaseNumber
        { get { lock (_lock) return _phase; } }

        /// <summary>参与者总数</summary>
        public int ParticipantCount => _participantCount;

        public void Dispose()
        {
            lock (_lock)
            {
                _tcs.TrySetCanceled();
            }
        }
    }
}