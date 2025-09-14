using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 框架级“类型错误”异常，用于替代 ArgumentException 中所有“类型不匹配”场景。
    /// </summary>
    [Serializable]
    public class TypeErrorException : Exception
    {
        /// <summary>
        /// 预期类型
        /// </summary>
        public Type ExpectedType { get; }

        /// <summary>
        /// 实际类型
        /// </summary>
        public Type ActualType { get; }

        #region 标准构造器

        /// <summary>
        /// 初始化 <see cref="TypeErrorException"/> 类的新实例。
        /// </summary>
        public TypeErrorException()
        { }

        /// <summary>
        /// 使用指定错误消息初始化 <see cref="TypeErrorException"/> 类的新实例。
        /// </summary>
        public TypeErrorException(string message) : base(message) { }

        /// <summary>
        /// 使用指定错误消息和内部异常初始化 <see cref="TypeErrorException"/> 类的新实例。
        /// </summary>
        public TypeErrorException(string message, Exception innerException) : base(message, innerException) { }

        #endregion 标准构造器

        #region 实用工厂构造

        /// <summary>
        /// 当实际类型与预期类型不符时抛出异常。
        /// </summary>
        /// <param name="expectedType">预期类型</param>
        /// <param name="actualType">实际类型</param>
        /// <param name="paramName">参数名（可选）</param>
        public static void ThrowIfMismatch(Type expectedType, Type actualType, string paramName = null)
        {
            if (expectedType == actualType) return;

            throw new TypeErrorException(
                paramName == null
                    ? $"类型不匹配：预期 '{expectedType.FullName}'，实际 '{actualType.FullName}'。"
                    : $"参数 '{paramName}' 类型不匹配：预期 '{expectedType.FullName}'，实际 '{actualType.FullName}'。",
                expectedType,
                actualType);
        }

        /// <summary>
        /// 当对象不是预期类型或其派生类型时抛出异常。
        /// </summary>
        /// <param name="expectedType">预期类型</param>
        /// <param name="instance">待检测实例</param>
        /// <param name="paramName">参数名（可选）</param>
        public static void ThrowIfNotAssignableFrom(Type expectedType, object instance, string paramName = null)
        {
            if (instance == null) return;   // 空引用由 ThrowIfNull 处理
            var actualType = instance.GetType();
            if (expectedType.IsAssignableFrom(actualType)) return;

            throw new TypeErrorException(
                paramName == null
                    ? $"类型必须继承或实现 '{expectedType.FullName}'，实际为 '{actualType.FullName}'。"
                    : $"参数 '{paramName}' 必须继承或实现 '{expectedType.FullName}'，实际为 '{actualType.FullName}'。",
                expectedType,
                actualType);
        }

        #endregion 实用工厂构造

        #region 完整构造器（含类型信息）

        /// <summary>
        /// 使用预期类型、实际类型和自定义消息初始化异常。
        /// </summary>
        public TypeErrorException(string message, Type expectedType, Type actualType) : base(message)
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }

        /// <summary>
        /// 使用预期类型、实际类型、自定义消息和内部异常初始化异常。
        /// </summary>
        public TypeErrorException(string message, Type expectedType, Type actualType, Exception innerException) : base(message, innerException)
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }

        #endregion 完整构造器（含类型信息）

        #region 序列化支持

#if NET8_0_OR_GREATER
        [Obsolete("BinaryFormatter is obsolete in .NET 8+")]
#endif

        protected TypeErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExpectedType = Type.GetType(info.GetString(nameof(ExpectedType)) ?? string.Empty);
            ActualType = Type.GetType(info.GetString(nameof(ActualType)) ?? string.Empty);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ExpectedType), ExpectedType?.AssemblyQualifiedName);
            info.AddValue(nameof(ActualType), ActualType?.AssemblyQualifiedName);
        }

        #endregion 序列化支持
    }
}