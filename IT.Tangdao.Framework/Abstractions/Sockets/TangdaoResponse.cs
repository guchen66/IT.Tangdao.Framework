using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Sockets
{
    public sealed class TangdaoResponse : ITangdaoResponse
    {
        private readonly ITangdaoChannel _channel;

        public TangdaoResponse(ITangdaoChannel channel)
        {
            _channel = channel;
            var s = ((TangdaoChannel)_channel).Socket;
            s.MessageReceived += (sender, msg) => Received?.Invoke(this, msg);
            s.ErrorOccurred += (sender, ex) => Error?.Invoke(this, ex);
        }

        public event EventHandler<string> Received;

        public event EventHandler<Exception> Error;
    }
}