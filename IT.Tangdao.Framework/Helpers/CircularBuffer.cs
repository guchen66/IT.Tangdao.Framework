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
        private int _index = -1;

        public CircularBuffer(IEnumerable<T> source)
        {
            _data = source?.ToArray() ?? throw new ArgumentNullException(nameof(source));
            if (_data.Length == 0) throw new ArgumentException("Sequence contains no elements", nameof(source));
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
            int i = Interlocked.Increment(ref _index);
            // 正确的取模计算：确保结果为正
            int actualIndex = (i % _data.Length + _data.Length) % _data.Length;
            return _data[actualIndex];
        }

        /// <summary>
        /// 前一个值
        /// </summary>
        /// <returns></returns>
        public T GetPrevious()
        {
            int i = Interlocked.Decrement(ref _index);
            // 正确的取模计算：确保结果为正
            int actualIndex = (i % _data.Length + _data.Length) % _data.Length;
            return _data[actualIndex];
        }

        /// <summary>
        /// 跟据条件获取元素
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T GetItem(Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return _data.FirstOrDefault(predicate);
        }

        public T Next => GetNext();

        public T Previous => GetPrevious();
    }
}