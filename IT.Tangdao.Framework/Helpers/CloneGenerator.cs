using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    public static class CloneGenerator<T>
    {
        private static readonly Func<T, T> _cloner = Gen();

        private static Func<T, T> Gen()
        {
            var p = Expression.Parameter(typeof(T));
            var memberInit = Expression.MemberInit(
                Expression.New(typeof(T)),
                typeof(T).GetProperties()
                         .Select(pr => Expression.Bind(pr, Expression.Property(p, pr)))
            );
            return Expression.Lambda<Func<T, T>>(memberInit, p).Compile();
        }

        public static T Clone(T src) => _cloner(src);
    }
}