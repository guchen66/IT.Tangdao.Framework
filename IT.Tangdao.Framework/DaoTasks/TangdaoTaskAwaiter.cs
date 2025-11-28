using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    public readonly struct TangdaoTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly TaskAwaiter<TResult> _awaiter;

        public TangdaoTaskAwaiter(Task<TResult> task, bool continueOnCapturedContext)
        {
            _awaiter = task.GetAwaiter();
        }

        public bool IsCompleted => _awaiter.IsCompleted;

        public TResult GetResult() => _awaiter.GetResult();

        public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

        public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);
    }
}