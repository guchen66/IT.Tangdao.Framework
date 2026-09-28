
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using System;
using System.Collections.Concurrent;
using System.Reflection;
namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 属性级生成规则责任链。
    /// <para>
    /// 链中<b>顺序即优先级</b>：从前到后遍历，第一个 <c>CanHandle</c> 返回 true 的规则接手，
    /// 后续规则不再参与。
    /// </para>
    /// <para>
    /// 默认链顺序及理由：
    /// </para>
    /// <list type="number">
    /// <item><description>显式默认值——用户写死的值优先级最高，无需解释；</description></item>
    /// <item><description>字符串长度——只认 string，影响面窄，可安全前置；</description></item>
    /// <item><description>数值范围——只认带特性的数值属性，且已排除自增主键；</description></item>
    /// <item><description>自增主键——排在数值范围之后，但数值范围已显式让位，二者不会冲突；</description></item>
    /// <item><description>命名模板——从注册表取词库；</description></item>
    /// <item><description>类型兜底——永远接手，负责把类型路由到对应生成器。</description></item>
    /// </list>
    /// <para>
    /// 链实例本身无状态（规则所需信息全部从属性特性和上下文获取），
    /// 因此这里用一份静态实例供所有线程共享。
    /// </para>
    /// </summary>
    public static class FakeRuleChain
    {
        /// <summary>默认规则链，顺序即优先级。</summary>
        private static readonly ITangdaoFakeRule[] _rules = new ITangdaoFakeRule[]
        {
            new DefaultValueRule(),
            new LengthRule(),
            new NumericRangeRule(),
            new PrimaryKeyIncrementRule(),
            new TemplateRule(),
            new TypeFallbackRule()
        };

        /// <summary>
        /// 获取默认规则链（只读用途，例如调试时查看优先级）。
        /// </summary>
        public static ITangdaoFakeRule[] Rules
        {
            get { return _rules; }
        }

        /// <summary>
        /// 取属性的"有效类型"：可空类型（<c>Nullable&lt;T&gt;</c>）解包为其底层类型 <c>T</c>，其余类型原样返回。
        /// <para>
        /// <b>为什么每条规则都必须用它</b>：<c>T?</c> 在反射下的实际类型是 <c>Nullable&lt;T&gt;</c>，
        /// 而这是一个"不可生成"的类型——注册表里只有 <c>T</c> 的生成器，没有也不该有 <c>Nullable&lt;T&gt;</c> 的；
        /// <c>Convert.ChangeType</c> 同样不接受 <c>Nullable&lt;T&gt;</c> 作为目标类型（它只认 IConvertible 的具体类型）。
        /// </para>
        /// <para>
        /// 如果各规则各写各的类型判断，就会出现典型的静默错误：带 <c>Min/Max</c> 的 <c>int?</c>
        /// 因类型不匹配而落到链尾兜底，兜底又查不到生成器，最终
        /// <c>Activator.CreateInstance(typeof(int?))</c> 返回 null——属性永远为 null，
        /// 却不会抛出任何异常，极难排查。统一走本方法后，"可空"这层语义只在门面层的
        /// 空值策略处裁决一次，规则层一律按底层类型处理，生成出的装箱值可直接赋给 <c>T?</c> 属性。
        /// </para>
        /// </summary>
        /// <param name="property">目标属性</param>
        public static Type GetEffectiveType(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        }

        /// <summary>
        /// 为单个属性裁决并生成值。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <param name="context">生成上下文</param>
        /// <returns>属性值；无规则接手时返回 null（正常不会发生，链尾规则永远接手）</returns>
        public static object ResolvePropertyValue(PropertyInfo property, TangdaoGeneratorContext context)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            for (int i = 0; i < _rules.Length; i++)
            {
                if (_rules[i].CanHandle(property))
                {
                    return _rules[i].Resolve(property, context);
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 属性特性缓存：缓存每个属性上的 <see cref="TangdaoFakeAttribute"/>。
    /// <para>
    /// 【关键】缓存键必须包含<b>声明类型的全名</b>。
    /// 旧实现把缓存放在泛型类 <c>TangdaoDataFaker&lt;T&gt;</c> 的静态字段里，且只用
    /// <c>property.Name</c> 作键，靠"泛型静态字段按封闭类型各存一份"这一隐式机制侥幸隔离；
    /// 一旦缓存被抽到非泛型层（本版正是如此），同名属性就会跨类型互相串味
    /// （例如 A 类与 B 类都有 <c>Name</c> 属性，A 的特性会被 B 读到）。
    /// 因此这里显式把 <c>DeclaringType.FullName</c> 纳入键。
    /// </para>
    /// <para>
    /// 特性在运行期不会变化，缓存可以永久有效，无需失效策略。
    /// </para>
    /// </summary>
    internal static class FakeAttributeCache
    {
        /// <summary>键为"类型全名.属性名"，值为该属性上的特性（可能为 null，null 也要缓存以避免反复反射）。</summary>
        private static readonly ConcurrentDictionary<string, TangdaoFakeAttribute> _cache =
            new ConcurrentDictionary<string, TangdaoFakeAttribute>();

        /// <summary>
        /// 取属性上的 <see cref="TangdaoFakeAttribute"/>，无则返回 null。
        /// </summary>
        /// <param name="property">目标属性</param>
        public static TangdaoFakeAttribute Get(PropertyInfo property)
        {
            if (property == null)
            {
                return null;
            }

            Type declaringType = property.DeclaringType;
            string typeName = declaringType == null ? string.Empty : declaringType.FullName;
            string key = typeName + "." + property.Name;

            return _cache.GetOrAdd(key, k => property.GetCustomAttribute<TangdaoFakeAttribute>());
        }
    }
}
