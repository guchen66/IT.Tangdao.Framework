using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Sockets
{
    // 1. 上下文 = 三件套持有者
    public sealed class TangdaoChannelContext
    {
        public ITangdaoChannel Channel { get; }
        public ITangdaoRequest Request { get; }
        public ITangdaoResponse Response { get; }

        internal TangdaoChannelContext(ITangdaoChannel ch, ITangdaoRequest req, ITangdaoResponse resp)
        {
            Channel = ch; Request = req; Response = resp;
        }

        /* 帮助方法：一次性连接 */

        public Task<bool> ConnectAsync(CancellationToken token = default) =>
            Channel.ConnectAsync(token);
    }
}