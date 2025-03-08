using IT.Tangdao.Framework.DaoDtos;
using IT.Tangdao.Framework.DaoDtos.Globals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoHardwares
{
    public class TangdaoMessageInvoker : IDisposable
    {
        private bool _disposed; //标记是否已经释放

        // 需要释放的资源
        private bool _disposeHandler;

        private IntPtr _unmanagedResource;
        private Stream _managedResource;
        private readonly TangdaoMessageHandler _handler;

        public TangdaoMessageInvoker(TangdaoMessageHandler handler) : this(handler, true)
        {
        }

        public TangdaoMessageInvoker(TangdaoMessageHandler handler, bool disposeHandler)
        {
            TangdaoGuards.ThrowIfNull(handler);
            _handler = handler;
            _disposeHandler = disposeHandler;
        }

        public virtual TangdaoResponse Send(TangdaoRequest request)
        {
            try
            {
                TangdaoResponse tangdaoResponse = null;
                tangdaoResponse = _handler.Send();
                return tangdaoResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<TangdaoResponse> SendAsync(TangdaoRequest request, CancellationToken token)
        {
            TangdaoGuards.ThrowIfNull(request);
            try
            {
                TangdaoResponse tangdaoResponse = null;
                tangdaoResponse = await _handler.SendAsync(request, token);
                return tangdaoResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 析构函数（终结器）
        ~TangdaoMessageInvoker()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // 抑制终结器
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // 释放托管资源
                _managedResource?.Dispose();
            }

            // 释放非托管资源
            if (_unmanagedResource != IntPtr.Zero)
            {
                // 释放非托管资源的代码
                _unmanagedResource = IntPtr.Zero;
            }

            _disposed = true;
        }
    }
}