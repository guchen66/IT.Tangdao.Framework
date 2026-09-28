
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Reflection;
namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 主键自增规则：负责 <c>[TangdaoFake(PrimarykeyAutoIncrement = true)]</c> 的整数键属性。
    /// <para>
    /// 【取值策略】本规则在<b>并发生成阶段只写入占位值 0</b>，真正的连续序号由门面
    /// <c>TangdaoDataFaker&lt;T&gt;.Build</c> 在所有对象生成完毕后的<b>串行收尾阶段</b>统一回填。
    /// </para>
    /// <para>
    /// 为什么这么设计：旧实现让每个并行线程各自去取全局自增计数器
    /// （<c>_intIdCounter++</c>），序号顺序取决于线程调度，只能事后整体重排，
    /// 而且那个计数器是<b>进程级静态字段</b>，多个调用方同时构建时会互相污染。
    /// 把序号生成从并发路径里彻底移出后，ID 天然连续且不受并发影响。
    /// </para>
    /// <para>
    /// 【与原实现的差异】旧实现要求"属性名含 Id"<b>且</b>"声明了自增"两个条件同时成立；
    /// 本版只要求"声明了自增 + 整数键类型"。理由：用户显式写下
    /// <c>PrimarykeyAutoIncrement = true</c> 时意图已经明确，不应再被属性命名卡住。
    /// </para>
    /// </summary>
    public sealed class PrimaryKeyIncrementRule : ITangdaoFakeRule
    {
        /// <summary>
        /// 属性是否为显式声明自增的整数键。
        /// </summary>
        /// <param name="property">待判断的属性</param>
        public bool CanHandle(PropertyInfo property)
        {
            return IsAutoIncrementKey(property);
        }

        /// <summary>
        /// 写入占位值（0），真实序号由 Build 串行收尾阶段回填。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <param name="context">生成上下文</param>
        public object Resolve(PropertyInfo property, TangdaoGeneratorContext context)
        {
            return Convert.ChangeType(0, property.PropertyType);
        }

        /// <summary>
        /// 判断属性是否为需要自增的主键。
        /// <para>可被其它规则或门面复用，以保证"谁是自增主键"这一判断全库口径一致。</para>
        /// </summary>
        /// <param name="property">待判断的属性</param>
        public static bool IsAutoIncrementKey(PropertyInfo property)
        {
            if (property == null)
            {
                return false;
            }

            TangdaoFakeAttribute attribute = FakeAttributeCache.Get(property);
            if (attribute == null || !attribute.PrimarykeyAutoIncrement)
            {
                return false;
            }

            // 解包后再判类型：int? 主键同样应当被识别为自增键
            return FakeRuleChain.GetEffectiveType(property).IsIntegerKey();
        }
    }
}
