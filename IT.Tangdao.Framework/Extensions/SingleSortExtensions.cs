using IT.Tangdao.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class SingleSortExtensions
    {
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, Func<T, IComparable> keySelector)
        {
            var sortProvider = new SingleSortUtils<T>(keySelector);
            return source.OrderBy(x => x, sortProvider);
        }
    }
}