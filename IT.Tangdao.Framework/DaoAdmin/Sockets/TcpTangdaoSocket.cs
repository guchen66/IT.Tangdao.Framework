using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.DaoParameters.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.SocketMessages
{
    public class TcpTangdaoSocket : TangdaoSocketBase
    {
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private CancellationTokenSource _receiveCts;

        public TcpTangdaoSocket(NetMode mode, ITangdaoUri uri) : base(mode, uri)
        {
        }

        public override async Task<bool> ConnectAsync()
        {
            try
            {
                _tcpClient = new TcpClient();

                if (Mode == NetMode.Client)
                {
                    await _tcpClient.ConnectAsync(Uri.Host, Uri.Port);
                }
                else // Server mode
                {
                    var listener = new TcpListener(IPAddress.Any, Uri.Port);
                    listener.Start();
                    _tcpClient = await listener.AcceptTcpClientAsync();
                }

                _networkStream = _tcpClient.GetStream();
                IsConnected = true;

                // Start receiving messages
                _receiveCts = new CancellationTokenSource();
                _ = StartReceivingAsync(_receiveCts.Token);

                return true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return false;
            }
        }

        private async Task StartReceivingAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            while (!cancellationToken.IsCancellationRequested && IsConnected)
            {
                try
                {
                    int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        OnMessageReceived(message);
                    }
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                    break;
                }
            }
        }

        public override async Task SendAsync(string message)
        {
            if (!IsConnected) return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            await _networkStream.WriteAsync(data, 0, data.Length);
        }

        public override async Task<string> ReceiveAsync()
        {
            var buffer = new byte[4096];
            int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public override async Task DisconnectAsync()
        {
            _receiveCts?.Cancel();
            _networkStream?.Close();
            _tcpClient?.Close();
            IsConnected = false;
            await Task.CompletedTask;
        }
    }
}