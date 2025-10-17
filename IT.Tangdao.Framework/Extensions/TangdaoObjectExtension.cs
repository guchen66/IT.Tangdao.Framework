using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoObjectExtension
    {
        public static T As<T>(this object obj) where T : class => obj as T;

        public static T AsOrFail<T>(this object obj, string expr = null) where T : class
            => obj is T t ? t
               : throw new InvalidCastException(
                   $"表达式 '{expr}' 的实际类型 {obj?.GetType().Name} 无法转换成 {typeof(T).Name}");

        public static bool TryAs<T>(this object obj, out T result) where T : class
        {
            result = obj as T;
            return result != null;
        }

        public static bool IsNot<T>(this object obj)
        {
            return !(obj is T);
        }
    }
}