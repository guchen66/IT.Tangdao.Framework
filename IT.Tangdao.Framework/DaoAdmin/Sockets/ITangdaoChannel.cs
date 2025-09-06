using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Sockets
{
    /// <summary>
    /// 建立连接通道
    /// </summary>
    public interface ITangdaoChannel : IDisposable
    {
        bool IsConnected { get; }

        Task<bool> ConnectAsync(CancellationToken token = default);

        /// <summary>
        /// 防止异步连接未完成的时候，界面却已经构建成功
        /// 所以，一定要等待连接在建立界面构造
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> WaitConnectedAsync(CancellationToken token = default);

        Task DisconnectAsync();
    }
}