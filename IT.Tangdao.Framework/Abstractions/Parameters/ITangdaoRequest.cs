using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public interface ITangdaoRequest : ITangdaoParameter
    {
        string Command { get; }                       // 强制定义
        TaskCompletionSource<object> ReplySource { get; }
    }
}