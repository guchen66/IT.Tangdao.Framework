using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDtos
{
    public abstract class TangdaoMessageHandler : IDisposable
    {
        protected internal virtual TangdaoResponse Send()
        {
            throw new NotImplementedException();
        }

        protected internal abstract Task<TangdaoResponse> SendAsync(TangdaoRequest request, CancellationToken token);

        protected virtual void Dispose(bool disposing)
        {
            // Nothing to do in base class.
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}