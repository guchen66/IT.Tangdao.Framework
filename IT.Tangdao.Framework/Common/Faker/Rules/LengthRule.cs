
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Utilities;
using System;
using System.Reflection;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 字符串长度规则：负责 <c>[TangdaoFake(Length = n)]</c>。
    /// <para>
    /// 只对 <c>string</c> 属性生效。若 Length 加在数值属性上，本规则不接手，
    /// 继续交给后续规则处理——这与旧实现"<c>Length &gt; 0 &amp;&amp; 类型是 string</c> 才生效"保持一致，
    /// 避免"给 int 属性写 Length"这种无效配置被静默当作长度处理。
    /// </para>
    /// </summary>
    public sealed class LengthRule : ITangdaoFakeRule
    {
        /// <summary>
        /// 属性是否为声明了正数 Length 的字符串。
        /// </summary>
        /// <param name="property">待判断的属性</param>
        public bool CanHandle(PropertyInfo property)
        {
            if (property.PropertyType != typeof(string))
            {
                return false;
            }

            TangdaoFakeAttribute attribute = FakeAttributeCache.Get(property);
            return attribute != null && attribute.Length > 0;
        }

        /// <summary>
        /// 生成指定长度的随机字符串。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <param name="context">生成上下文</param>
        public object Resolve(PropertyInfo property, TangdaoGeneratorContext context)
        {
            TangdaoFakeAttribute attribute = FakeAttributeCache.Get(property);
            return FakedataUtils.GenerateRandomString(context.Random, attribute.Length);
        }
    }
}
