
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using System;
using System.Reflection;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 显式默认值规则：链中优先级最高的规则，负责 <c>[TangdaoFake(DefaultValue = "...")]</c>。
    /// <para>
    /// 语义：只要属性显式声明了 DefaultValue，就无条件产出该值，
    /// 后面的规则不再参与。这符合"用户显式指定优先于任何自动推断"的原则。
    /// </para>
    /// <para>
    /// 注意：DefaultValue 是 <c>string</c> 类型，需要用 <c>Convert.ChangeType</c> 转到属性实际类型。
    /// 转换失败（例如给 int 属性写了 <c>DefaultValue = "abc"</c>）时返回 null，
    /// 由上层跳过赋值，而不是让整个生成过程因一个属性配置错误而崩掉。
    /// </para>
    /// </summary>
    public sealed class DefaultValueRule : ITangdaoFakeRule
    {
        /// <summary>
        /// 属性是否声明了非空 DefaultValue。
        /// </summary>
        /// <param name="property">待判断的属性</param>
        public bool CanHandle(PropertyInfo property)
        {
            TangdaoFakeAttribute attribute = FakeAttributeCache.Get(property);
            return attribute != null && !string.IsNullOrEmpty(attribute.DefaultValue);
        }

        /// <summary>
        /// 把 DefaultValue 字符串转换为属性类型后返回。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <param name="context">生成上下文</param>
        public object Resolve(PropertyInfo property, TangdaoGeneratorContext context)
        {
            TangdaoFakeAttribute attribute = FakeAttributeCache.Get(property);

            try
            {
                // 用有效类型作为转换目标：Convert.ChangeType 不接受 Nullable<T>，
                // 对 int? 属性直接转会抛异常、被下面的 catch 吞成 null
                return Convert.ChangeType(attribute.DefaultValue, FakeRuleChain.GetEffectiveType(property));
            }
            catch (Exception)
            {
                // 配置错误不应中断整批数据生成，返回 null 由上层跳过该属性
                return null;
            }
        }
    }
}
