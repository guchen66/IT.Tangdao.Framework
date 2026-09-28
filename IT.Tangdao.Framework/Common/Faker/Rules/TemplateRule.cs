
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Utilities;
using System;
using System.Reflection;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 命名模板规则：负责 <c>[TangdaoFake(Template = "...")]</c>。
    /// <para>
    /// 模板不再用 <c>switch</c> 硬编码在工具类里，而是从
    /// <see cref="ITangdaoGeneratorRegistry"/> 按名字查工厂，
    /// 因此调用方可以注册自己的词库（"产品编号""部门名称"等）而无需改库源码。
    /// </para>
    /// <para>
    /// 注意：旧的 <c>GetRandomTemplateValue</c> 有一个隐藏缺陷——<c>switch</c> 遗漏了
    /// <c>Hobby</c> 分支，导致 <c>Template = MockTemplate.Hobby</c> 会静默落到 default 返回随机串。
    /// 本版由注册表统一登记六个模板，从结构上消除了这类遗漏。
    /// </para>
    /// </summary>
    public sealed class TemplateRule : ITangdaoFakeRule
    {
        /// <summary>未命中模板且属性为字符串时的兜底长度。</summary>
        private const int FallbackLength = 6;

        /// <summary>
        /// 属性是否声明了非空 Template。
        /// </summary>
        /// <param name="property">待判断的属性</param>
        public bool CanHandle(PropertyInfo property)
        {
            TangdaoFakeAttribute attribute = FakeAttributeCache.Get(property);
            return attribute != null && !string.IsNullOrEmpty(attribute.Template);
        }

        /// <summary>
        /// 按模板名从注册表取工厂并执行。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <param name="context">生成上下文</param>
        public object Resolve(PropertyInfo property, TangdaoGeneratorContext context)
        {
            TangdaoFakeAttribute attribute = FakeAttributeCache.Get(property);

            Func<TangdaoGeneratorContext, object> factory;
            if (context.Registry != null && context.Registry.TryResolveTemplate(attribute.Template, out factory) && factory != null)
            {
                return factory(context);
            }

            // 模板名写错时的降级：字符串属性给一个随机串，其它类型返回 null 交由上层跳过，
            // 保证"模板名拼错"不会让整批生成失败
            if (FakeRuleChain.GetEffectiveType(property) == typeof(string))
            {
                return FakedataUtils.GenerateRandomString(context.Random, FallbackLength);
            }

            return null;
        }
    }
}
