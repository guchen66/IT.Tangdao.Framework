using IT.Tangdao.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 浅拷贝快速复制类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class CloneGenerator<T>
    {
        private static readonly Func<T, T> _cloner = Gen();

        private static Func<T, T> Gen()
        {
            var p = Expression.Parameter(typeof(T));

            // 过滤掉有 IgnoreAttribute 的属性
            var properties = typeof(T).GetProperties()
                .Where(pr => !pr.IsDefined(typeof(IgnoreAttribute), false));

            var memberInit = Expression.MemberInit(
                Expression.New(typeof(T)),
                properties.Select(pr => Expression.Bind(pr, Expression.Property(p, pr)))
            );

            return Expression.Lambda<Func<T, T>>(memberInit, p).Compile();
        }

        public static T Clone(T src) => _cloner(src);
    }
}