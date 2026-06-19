using IT.Tangdao.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 枚举之间转换的泛型工具类，支持两个枚举类型之间的无缝转换
    /// </summary>
    /// <typeparam name="TSource">源枚举类型</typeparam>
    /// <typeparam name="TTarget">目标枚举类型</typeparam>
    public static class EnumToEnumParse<TSource, TTarget> where TSource : struct, Enum where TTarget : struct, Enum
    {
        /// <summary>
        /// 按枚举底层数值进行转换
        /// </summary>
        /// <param name="source">源枚举值</param>
        /// <returns>转换后的目标枚举值</returns>
        /// <exception cref="InvalidOperationException">当无法转换时抛出</exception>
        public static TTarget Converter(TSource source)
        {
            return EnumUtils<TSource, TTarget>.Convert(source);
        }
    }
}