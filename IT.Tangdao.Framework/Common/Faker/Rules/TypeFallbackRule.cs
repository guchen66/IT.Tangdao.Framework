
using IT.Tangdao.Framework.Abstractions.Contracts;
using System;
using System.Reflection;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 类型兜底规则：责任链的最后一环，负责"前面所有规则都不接手"的属性。
    /// <para>
    /// 它不做任何策略判断，只把工作转交给注册表中匹配该类型的
    /// <see cref="ITangdaoValueGenerator"/>。换言之：<b>类型→值 的全部知识都在生成器里，
    /// 本规则只负责"路由"</b>。这样新增一种类型支持时，只需注册一个生成器，
    /// 规则层零改动。
    /// </para>
    /// </summary>
    public sealed class TypeFallbackRule : ITangdaoFakeRule
    {
        /// <summary>
        /// 永远接手（链尾兜底）。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        public bool CanHandle(PropertyInfo property)
        {
            return true;
        }

        /// <summary>
        /// 从注册表查生成器并生成值；查不到时回退为类型默认值。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <param name="context">生成上下文</param>
        public object Resolve(PropertyInfo property, TangdaoGeneratorContext context)
        {
            // 必须先解包可空类型：注册表里只有 T 的生成器，没有 Nullable<T> 的。
            // 不解包的话 int? 属性会一路兜底成 null，且不报任何错。
            Type type = FakeRuleChain.GetEffectiveType(property);

            ITangdaoValueGenerator generator;
            if (context.Registry != null && context.Registry.TryResolve(type, out generator) && generator != null)
            {
                return generator.Generate(type, context);
            }

            // 兜底兜不住（既没有生成器、也不是可构造类型）：
            // 值类型给默认值保证赋值合法，引用类型给 null
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
