using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Parameters.EventArg;
using IT.Tangdao.Framework.Parameters.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.SocketMessages
{
    public abstract class TangdaoSocketBase : ITangdaoSocket
    {
        public NetMode Mode { get; protected set; }
        public NetConnectionType ConnectionType { get; protected set; }
        public bool IsConnected { get; protected set; }
        public ITangdaoUri Uri { get; protected set; }

        public event EventHandler<string> MessageReceived;

        public event EventHandler<Exception> ErrorOccurred;

        protected TangdaoSocketBase(NetMode mode, ITangdaoUri uri)
        {
            Mode = mode;
            Uri = uri;
            ConnectionType = uri.ConnectionType;
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

        protected virtual void OnErrorOccurred(Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
        }

        public abstract Task<bool> ConnectAsync();

        public abstract Task DisconnectAsync();

        public abstract Task SendAsync(string message);

        public abstract Task<string> ReceiveAsync();
    }
}