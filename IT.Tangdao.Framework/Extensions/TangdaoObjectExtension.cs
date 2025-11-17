using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    using System;

    /// <summary>
    /// 提供对象类型转换和类型检查的扩展方法。
    /// 这些方法简化了常见的类型操作，提供了安全且易用的类型转换方式。
    /// </summary>
    public static class TangdaoObjectExtension
    {
        /// <summary>
        /// 将对象安全地转换为指定类型。如果转换失败则返回 null。
        /// </summary>
        /// <typeparam name="T">要转换的目标类型，必须是引用类型。</typeparam>
        /// <param name="obj">要转换的对象。</param>
        /// <returns>转换后的对象，如果转换失败则返回 null。</returns>
        public static T As<T>(this object obj) where T : class
        {
            return obj as T;
        }

        /// <summary>
        /// 将对象转换为指定类型，如果转换失败则抛出 <see cref="InvalidCastException"/>。
        /// </summary>
        /// <typeparam name="T">要转换的目标类型，必须是引用类型。</typeparam>
        /// <param name="obj">要转换的对象。</param>
        /// <param name="expression">可选的表达式名称，用于在异常消息中提供更多上下文信息。</param>
        /// <returns>转换后的对象。</returns>
        public static T AsOrFail<T>(this object obj, string expression = null) where T : class
        {
            if (obj is T result)
            {
                return result;
            }

            string objTypeName = obj?.GetType().Name ?? "null";
            string message = string.IsNullOrEmpty(expression)
                ? $"无法将类型 {objTypeName} 转换为 {typeof(T).Name}。"
                : $"表达式 '{expression}' 的实际类型 {objTypeName} 无法转换成 {typeof(T).Name}。";

            throw new InvalidCastException(message);
        }

        /// <summary>
        /// 尝试将对象转换为指定类型，并返回指示转换是否成功的值。
        /// </summary>
        /// <typeparam name="T">要转换的目标类型，必须是引用类型。</typeparam>
        /// <param name="obj">要转换的对象。</param>
        /// <param name="result">当此方法返回时，如果转换成功，则包含转换后的对象；否则为 null。</param>
        /// <returns>如果对象可转换为目标类型，则为 true；否则为 false。</returns>
        public static bool TryAs<T>(this object obj, out T result) where T : class
        {
            result = obj as T;
            return result != null;
        }

        /// <summary>
        /// 确定对象是否不是指定类型。
        /// </summary>
        /// <typeparam name="T">要检查的类型。</typeparam>
        /// <param name="obj">要检查的对象。</param>
        /// <returns>如果对象不是指定类型，则为 true；否则为 false。</returns>
        public static bool IsNot<T>(this object obj)
        {
            return !(obj is T);
        }
    }
}