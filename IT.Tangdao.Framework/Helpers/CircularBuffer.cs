using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 线程安全的泛型循环缓冲
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CircularBuffer<T>
    {
        private readonly T[] _data;
        private int _index = 0;

        public CircularBuffer(IEnumerable<T> source)
        {
            _data = source?.ToArray() ?? throw new ArgumentNullException(nameof(source));
            if (_data.Length == 0) throw new ArgumentException("Sequence contains no elements", nameof(source));
        }

        public int Count => _data.Length;

        public T GetNext()
        {
            var i = Interlocked.Increment(ref _index) - 1;
            return _data[i % _data.Length];
        }

        public T GetPrevious()
        {
            var i = Interlocked.Decrement(ref _index) - 1;
            return _data[(i + _data.Length) % _data.Length];
        }

        public T Next => GetNext();

        public T Previous => GetPrevious();
    }
}