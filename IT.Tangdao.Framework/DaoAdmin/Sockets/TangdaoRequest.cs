using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Sockets
{
    public sealed class TangdaoRequest : ITangdaoRequest
    {
        private readonly ITangdaoChannel _channel;

        public TangdaoRequest(ITangdaoChannel channel) => _channel = channel;

        public Task SendAsync(string message, CancellationToken token = default) =>
            ((TangdaoChannel)_channel).Socket.SendAsync(message);
    }
}