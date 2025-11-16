using IT.Tangdao.Framework.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 将一个类型转成列表
    /// 例如Student，List<Student>
    /// </summary>
    public static class OnlyObjectConverterExtension
    {
        // 真正的单例容器
        private static class SingletonList<T>
        {
            internal static readonly List<T> Instance = new List<T>(1);

            static SingletonList() => Instance.Add(default);
        }

        // 返回只读单例列表，0 分配、0 拷贝
        public static List<T> ToSingletonList<T>(this T item) where T : IOnlyObject
            => SingletonList<T>.Instance;

        // 单例列表 + 投射
        public static List<TResult> ToSingletonList<T, TResult>(this T item, Func<T, TResult> selector) where T : IOnlyObject
        {
            var result = SingletonList<TResult>.Instance;
            result[0] = selector(item);
            return result;
        }

        /// <summary>
        /// 将单个对象转换为只读列表
        /// </summary>
        public static IReadOnlyList<T> ToReadOnlyList<T>(this T item) where T : IOnlyObject
        {
            return item == null ? Array.Empty<T>() : new[] { item };
        }
    }
}