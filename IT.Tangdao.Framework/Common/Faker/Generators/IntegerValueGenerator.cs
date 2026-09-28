
using IT.Tangdao.Framework.Abstractions.Contracts;
using System;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 整数族生成器：统一负责 <c>int / long / short / byte / sbyte / ushort / uint / ulong</c>。
    /// <para>
    /// 为什么合并成一个类而不是八个：这八种类型的造值逻辑完全同构（区间内取随机整数再转换），
    /// 拆开只会产生八份近似重复的代码。本库的原则是"职责单一，但不为拆而拆"。
    /// </para>
    /// <para>
    /// 内部统一按 <c>int</c> 取值，再 <c>Convert.ChangeType</c> 到目标类型；
    /// 对 <c>byte / sbyte</c> 这类窄类型会自动收敛上界，避免转换时溢出。
    /// </para>
    /// </summary>
    public sealed class IntegerValueGenerator : ITangdaoValueGenerator
    {
        /// <summary>随机整数下界（含）。</summary>
        private const int MinValue = 1;

        /// <summary>随机整数默认上界（不含）。</summary>
        private const int MaxValue = 1000;

        /// <summary>
        /// 判断是否为整数族类型。
        /// </summary>
        /// <param name="type">目标类型</param>
        public bool CanGenerate(Type type)
        {
            return type == typeof(int)
                || type == typeof(long)
                || type == typeof(short)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong);
        }

        /// <summary>
        /// 生成一个随机整数并转换为目标类型。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="context">生成上下文</param>
        public object Generate(Type type, TangdaoGeneratorContext context)
        {
            int value = context.Next(MinValue, GetSafeUpperBound(type));
            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 取目标类型的安全上界。
        /// <para>窄类型必须收敛，否则 Convert.ChangeType 会抛 OverflowException：
        /// byte 只能容纳 0~255、sbyte 只能容纳 -128~127。</para>
        /// </summary>
        /// <param name="type">目标类型</param>
        private static int GetSafeUpperBound(Type type)
        {
            if (type == typeof(byte))
            {
                return 256;
            }

            if (type == typeof(sbyte))
            {
                return 128;
            }

            return MaxValue;
        }
    }
}
