using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 当属性/字段/参数要求指定长度，而实际长度不符时抛出。
    /// </summary>
    [Serializable]
    public class NotLengthException : Exception
    {
        /// <summary>
        /// 期望长度
        /// </summary>
        public int ExpectedLength { get; }

        /// <summary>
        /// 实际长度
        /// </summary>
        public int ActualLength { get; }

        #region 标准构造器

        public NotLengthException()
        { }

        public NotLengthException(string message) : base(message)
        {
        }

        public NotLengthException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion 标准构造器

        #region 实用工厂

        /// <summary>
        /// 长度不符时抛出
        /// </summary>
        /// <param name="expected">期望长度</param>
        /// <param name="actual">实际长度</param>
        /// <param name="paramName">参数名（可选）</param>
        public static void ThrowIfMismatch(int expected, int actual, string paramName = null)
        {
            if (expected == actual) return;
            throw new NotLengthException(
                paramName == null
                    ? $"长度必须等于 {expected}，实际 {actual}。"
                    : $"参数 '{paramName}' 长度必须等于 {expected}，实际 {actual}。",
                expected, actual);
        }

        /// <summary>
        /// 字符串长度不符时抛出
        /// </summary>
        public static void ThrowIfMismatch(string value, int expected, string paramName = null)
        {
            if (value == null) return;          // 空字符串由 ThrowIfNull 处理
            ThrowIfMismatch(expected, value.Length, paramName);
        }

        #endregion 实用工厂

        #region 完整构造器

        public NotLengthException(string message, int expectedLength, int actualLength)
            : base(message)
        {
            ExpectedLength = expectedLength;
            ActualLength = actualLength;
        }

        public NotLengthException(string message, int expectedLength, int actualLength, Exception inner)
            : base(message, inner)
        {
            ExpectedLength = expectedLength;
            ActualLength = actualLength;
        }

        #endregion 完整构造器

        #region 序列化支持

#if NET8_0_OR_GREATER
        [Obsolete("BinaryFormatter is obsolete in .NET 8+")]
#endif

        protected NotLengthException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ExpectedLength = info.GetInt32(nameof(ExpectedLength));
            ActualLength = info.GetInt32(nameof(ActualLength));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ExpectedLength), ExpectedLength);
            info.AddValue(nameof(ActualLength), ActualLength);
        }

        #endregion 序列化支持
    }
}