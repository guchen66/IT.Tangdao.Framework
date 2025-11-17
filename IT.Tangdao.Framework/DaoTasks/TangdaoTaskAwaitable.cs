using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    public readonly struct TangdaoTaskAwaitable<TResult>
    {
        public TangdaoTaskAwaitable<TResult>.Configure GetAwaiter()
        {
            return default;
        }

        public readonly struct Configure
        {
        }
    }
}