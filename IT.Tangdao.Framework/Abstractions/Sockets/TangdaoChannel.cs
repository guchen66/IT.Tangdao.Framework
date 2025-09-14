using IT.Tangdao.Framework.Abstractions.SocketMessages;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Parameters.EventArg;
using IT.Tangdao.Framework.Parameters.Infrastructure;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Sockets
{
    public sealed class TangdaoChannel : ITangdaoChannel
    {
        private readonly ITangdaoSocket _socket;
        private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        public TangdaoChannel(NetMode mode, ITangdaoUri uri)
        {
            _socket = new TangdaoSocketFactory().CreateSocket(mode, uri);
        }

        public bool IsConnected => _socket.IsConnected;

        /* 原来方法保持原样，只把连接结果写进 _tcs */

        public async Task<bool> ConnectAsync(CancellationToken token = default)
        {
            if (_tcs.Task.IsCompleted) return _tcs.Task.Result; // 已连过直接返回

            bool ok = false;
            try
            {
                ok = await _socket.ConnectAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _tcs.TrySetException(ex);
                return false;
            }
            _tcs.TrySetResult(ok);
            return ok;
        }

        public Task DisconnectAsync() => _socket.DisconnectAsync();

        public void Dispose()
        {
            (_socket as IDisposable)?.Dispose();
            _tcs.TrySetCanceled();
        }

        internal ITangdaoSocket Socket => _socket;

        /* 新增：等待连接完成（4.8 无 WaitAsync，手工取消） */

        public Task<bool> WaitConnectedAsync(CancellationToken token = default)
        {
            return _tcs.Task.IsCompleted ? _tcs.Task : WaitCoreAsync(token);
        }

        private async Task<bool> WaitCoreAsync(CancellationToken token)
        {
            var tcsWait = new TaskCompletionSource<bool>();
            using (token.Register(() => tcsWait.TrySetCanceled()))
            {
                return await (await Task.WhenAny(_tcs.Task, tcsWait.Task).ConfigureAwait(false))
                                      .ConfigureAwait(false);
            }
        }
    }
}