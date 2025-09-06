using IT.Tangdao.Framework.DaoAdmin.SocketMessages;
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

namespace IT.Tangdao.Framework.DaoAdmin.Sockets
{
    public class UdpTangdaoSocket : TangdaoSocketBase
    {
        private UdpClient _udpClient;
        private IPEndPoint _remoteEndPoint;
        private CancellationTokenSource _receiveCts;

        public UdpTangdaoSocket(NetMode mode, ITangdaoUri uri) : base(mode, uri)
        {
        }

        public override async Task<bool> ConnectAsync()
        {
            try
            {
                if (Mode == NetMode.Server)
                {
                    // 服务端模式
                    _udpClient = new UdpClient(Uri.Port);
                    Console.WriteLine($"UDP服务器启动在端口 {Uri.Port}");
                }
                else
                {
                    // 客户端模式
                    _udpClient = new UdpClient();
                    _remoteEndPoint = new IPEndPoint(IPAddress.Parse(Uri.Host), Uri.Port);
                    Console.WriteLine($"UDP客户端准备连接到 {Uri.Host}:{Uri.Port}");
                }

                IsConnected = true;

                // 启动接收线程
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
            while (!cancellationToken.IsCancellationRequested && IsConnected)
            {
                try
                {
                    UdpReceiveResult result;
                    if (Mode == NetMode.Server)
                    {
                        // 服务端接收任何来源的数据
                        result = await _udpClient.ReceiveAsync();
                        _remoteEndPoint = result.RemoteEndPoint; // 记录客户端地址
                    }
                    else
                    {
                        // 客户端接收数据
                        result = await _udpClient.ReceiveAsync();
                    }

                    string message = Encoding.UTF8.GetString(result.Buffer);
                    OnMessageReceived(message);
                }
                catch (ObjectDisposedException)
                {
                    // Socket被关闭，正常退出
                    break;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                }
            }
        }

        public override async Task SendAsync(string message)
        {
            if (!IsConnected) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);

                if (Mode == NetMode.Server)
                {
                    if (_remoteEndPoint != null)
                    {
                        await _udpClient.SendAsync(data, data.Length, _remoteEndPoint);
                    }
                }
                else
                {
                    await _udpClient.SendAsync(data, data.Length, _remoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        public override async Task<string> ReceiveAsync()
        {
            if (!IsConnected) return string.Empty;

            try
            {
                var result = await _udpClient.ReceiveAsync();
                return Encoding.UTF8.GetString(result.Buffer);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return string.Empty;
            }
        }

        public override async Task DisconnectAsync()
        {
            _receiveCts?.Cancel();
            _udpClient?.Close();
            IsConnected = false;
            await Task.CompletedTask;
        }
    }
}