using IT.Tangdao.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 线程安全的泛型循环缓冲
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CircularBuffer<T>
    {
        private readonly T[] _data;
        private int _index = -1;
        private volatile int _first = 1;  // 1=未初始化，0=已初始化

        // 主构造器：接受数组
        public CircularBuffer(T[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            if (_data.Length == 0)
                throw new ArgumentException("Sequence contains no elements", nameof(data));
        }

        // 从 IEnumerable 构造，复用数组构造器
        public CircularBuffer(IEnumerable<T> data)
            : this(data?.ToArray() ?? throw new ArgumentNullException(nameof(data)))
        {
            // 验证已在主构造器中完成
        }

        /// <summary>
        /// 缓冲区数量
        /// </summary>
        public int Count => _data.Length;

        /// <summary>
        /// 下一个值
        /// </summary>
        /// <returns></returns>
        public T GetNext()
        {
            if (Interlocked.CompareExchange(ref _first, 0, 1) == 1)
            {
                // 仅第一个线程进入
                _index = 0;
                return _data[0];
            }
            int i = Interlocked.Increment(ref _index);
            int actualIndex = (i % _data.Length + _data.Length) % _data.Length;
            return _data[actualIndex];
        }

        /// <summary>
        /// 前一个值
        /// </summary>
        /// <returns></returns>
        public T GetPrevious()
        {
            if (Interlocked.CompareExchange(ref _first, 0, 1) == 1)
            {
                _index = _data.Length - 1;
                return _data[_index];
            }
            int i = Interlocked.Decrement(ref _index);
            int actualIndex = (i % _data.Length + _data.Length) % _data.Length;
            return _data[actualIndex];
        }

        /// <summary>
        /// 跟据条件获取元素
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T GetItem(Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                TangdaoGuards.ThrowIfNull(nameof(predicate));
            }

            return _data.FirstOrDefault(predicate);
        }

        public T Next => GetNext();

        public T Previous => GetPrevious();
    }
}