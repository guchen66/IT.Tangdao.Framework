using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.DaoParameters.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.SocketMessages
{
    public interface ITangdaoSocket
    {
        NetMode Mode { get; }
        NetConnectionType ConnectionType { get; }
        bool IsConnected { get; }
        ITangdaoUri Uri { get; }

        Task<bool> ConnectAsync();

        Task DisconnectAsync();

        Task SendAsync(string message);

        Task<string> ReceiveAsync();

        event EventHandler<string> MessageReceived;

        event EventHandler<Exception> ErrorOccurred;
    }
}