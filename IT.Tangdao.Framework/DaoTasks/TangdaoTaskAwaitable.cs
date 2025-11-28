using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    public readonly struct TangdaoTaskAwaitable<TResult>
    {
        private readonly Task<TResult> _task;
        private readonly bool _continueOnCapturedContext;

        public TangdaoTaskAwaitable(Task<TResult> task, bool continueOnCapturedContext = true)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public TangdaoTaskAwaiter<TResult> GetAwaiter()
        {
            return new TangdaoTaskAwaiter<TResult>(_task, _continueOnCapturedContext);
        }
    }
}