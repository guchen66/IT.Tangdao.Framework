
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Utilities;
using System;
using System.Reflection;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 数值范围规则：负责 <c>[TangdaoFake(Min = a, Max = b, Point = p)]</c> 声明的数值属性。
    /// <para>
    /// 覆盖 <c>int / long / float / double / decimal</c>。为什么只处理"带特性的数值属性"：
    /// 未声明 Min/Max 的数值属性应走 <see cref="TypeFallbackRule"/> 的库默认区间，
    /// 这样"用户配置"与"库默认"两条路径保持清晰，不会互相覆盖。
    /// </para>
    /// <para>
    /// 【重要】自增主键属性要从本规则<b>让位</b>：<c>PrimarykeyAutoIncrement</c> 的属性取值由
    /// <see cref="PrimaryKeyIncrementRule"/> 负责。之所以在这里做排除而不是调整链顺序，
    /// 是因为自增属性同样带着 Min/Max 默认值，若不排除会被本规则抢先命中，
    /// 导致自增语义永远失效。
    /// </para>
    /// </summary>
    public sealed class NumericRangeRule : ITangdaoFakeRule
    {
        /// <summary>
        /// 属性是否为"带特性声明、且非自增主键"的数值类型。
        /// </summary>
        /// <param name="property">待判断的属性</param>
        public bool CanHandle(PropertyInfo property)
        {
            // 按解包后的有效类型判断：否则 int? 属性会被判为"不支持区间"，
            // 带着 Min/Max 配置落到链尾兜底，最终静默变成 null
            if (!IsRangeSupported(FakeRuleChain.GetEffectiveType(property)))
            {
                return false;
            }

            // 自增主键交给 PrimaryKeyIncrementRule，避免被范围随机抢走
            if (PrimaryKeyIncrementRule.IsAutoIncrementKey(property))
            {
                return false;
            }

            return FakeAttributeCache.Get(property) != null;
        }

        /// <summary>
        /// 按属性类型走对应的区间生成通道。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <param name="context">生成上下文</param>
        public object Resolve(PropertyInfo property, TangdaoGeneratorContext context)
        {
            TangdaoFakeAttribute attribute = FakeAttributeCache.Get(property);
            Type type = FakeRuleChain.GetEffectiveType(property);

            if (type == typeof(int))
            {
                return FakedataUtils.GenerateUniqueId(context.Random, attribute.Min, attribute.Max);
            }

            if (type == typeof(long))
            {
                return (long)FakedataUtils.GenerateUniqueId(context.Random, attribute.Min, attribute.Max);
            }

            if (type == typeof(float) || type == typeof(double))
            {
                return FakedataUtils.GenerateDoubleUniqueId(context.Random, attribute.Min, attribute.Max, attribute.Point);
            }

            if (type == typeof(decimal))
            {
                return FakedataUtils.GenerateDecimalUniqueId(context.Random, attribute.Min, attribute.Max, attribute.Point);
            }

            // CanHandle 已限定数值类型，正常不会走到这里
            return null;
        }

        /// <summary>
        /// 判断是否为本规则支持区间配置的数值类型。
        /// <para>刻意不复用类型扩展里的 <c>IsNumericType</c>：那个方法还包含 byte/short/uint 等，
        /// 而本规则只实现了 int/long/float/double/decimal 五条通道，两者范围必须一致。</para>
        /// </summary>
        /// <param name="type">目标类型</param>
        private static bool IsRangeSupported(Type type)
        {
            return type == typeof(int)
                || type == typeof(long)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal);
        }
    }
}
