using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Utilities
{
    internal class SingleSortUtils<T> : IComparer<T>
    {
        private readonly Func<T, IComparable> _keySelector;

        public SingleSortUtils(Func<T, IComparable> keySelector)
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public int Compare(T x, T y)
        {
            var keyX = _keySelector(x);
            var keyY = _keySelector(y);
            return keyX.CompareTo(keyY);
        }
    }
}