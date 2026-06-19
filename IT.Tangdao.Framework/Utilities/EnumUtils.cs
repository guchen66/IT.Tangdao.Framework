using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Utilities
{
    /// <summary>
    /// 枚举之间转换的泛型工具类，支持两个枚举类型之间的无缝转换
    /// </summary>
    /// <typeparam name="TSource">源枚举类型</typeparam>
    /// <typeparam name="TTarget">目标枚举类型</typeparam>
    internal static class EnumUtils<TSource, TTarget> where TSource : struct, Enum where TTarget : struct, Enum
    {
        public static readonly Func<TSource, TTarget> Convert;

        static EnumUtils()
        {
            var sourceParam = Expression.Parameter(typeof(TSource), "source");

            // 将源枚举转换为底层类型（如 int）
            var toUnderlying = Expression.Convert(sourceParam,
                Enum.GetUnderlyingType(typeof(TSource)));

            // 再转换为目标枚举类型
            var toTarget = Expression.Convert(toUnderlying, typeof(TTarget));

            // 编译成原生委托
            var lambda = Expression.Lambda<Func<TSource, TTarget>>(toTarget, sourceParam);
            Convert = lambda.Compile();
        }
    }
}