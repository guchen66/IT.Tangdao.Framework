
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Utilities;
using System;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 浮点族生成器：统一负责 <c>float / double / decimal</c>。
    /// <para>
    /// 产出 [0,1000) 区间内、保留 4 位小数的随机值。之所以把三者放一起，
    /// 是为了让同一批假数据在小数精度上保持同一口径，不会出现 float 与 double 精度不一致的情况。
    /// </para>
    /// <para>
    /// <c>decimal</c> 单独走 decimal 通道，避免先转 double 再转 decimal 引入二进制浮点误差。
    /// </para>
    /// </summary>
    public sealed class FloatingValueGenerator : ITangdaoValueGenerator
    {
        /// <summary>随机数下界（含）。</summary>
        private const int MinValue = 0;

        /// <summary>随机数上界（不含）。</summary>
        private const int MaxValue = 1000;

        /// <summary>保留的小数位数。</summary>
        private const int Point = 4;

        /// <summary>
        /// 判断是否为浮点族类型。
        /// </summary>
        /// <param name="type">目标类型</param>
        public bool CanGenerate(Type type)
        {
            return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        /// <summary>
        /// 生成一个带小数的随机数并转换为目标类型。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="context">生成上下文</param>
        public object Generate(Type type, TangdaoGeneratorContext context)
        {
            // decimal 走独立通道，避免 double 中转带来的精度损失
            if (type == typeof(decimal))
            {
                return FakedataUtils.GenerateDecimalUniqueId(context.Random, MinValue, MaxValue, Point);
            }

            double value = FakedataUtils.GenerateDoubleUniqueId(context.Random, MinValue, MaxValue, Point);
            return Convert.ChangeType(value, type);
        }
    }
}
