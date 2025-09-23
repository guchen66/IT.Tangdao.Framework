using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    public sealed class TangdaoScope : ITangdaoScope
    {
        private readonly List<object> _disposables = new List<object>();
        private bool _disposed;

        public ITangdaoProvider ScopedProvider { get; }

        public TangdaoScope(ITangdaoProvider scopedProvider)
        {
            ScopedProvider = scopedProvider ?? throw new ArgumentNullException(nameof(scopedProvider));
        }

        public void TrackForDispose(object instance)
        {
            if (instance is IDisposable d) _disposables.Add(d);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                var d = _disposables[i] as IDisposable;
                d?.Dispose();
            }
            _disposables.Clear();
        }
    }
}