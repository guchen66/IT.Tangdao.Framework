using IT.Tangdao.Framework.Abstractions.Contracts;
using System.Reflection;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 按概率裁决的空值策略：可空属性以 <see cref="TangdaoFakerOptions.NullableNullProbability"/>
    /// 的概率产出 null，其余情况产出有效值。
    /// <para>
    /// 这是库的默认策略，也是"<c>T?</c> 表示既允许 null 也允许有效值"这一语义最自然的落地方式。
    /// </para>
    /// <para>
    /// 边界处理：概率 &lt;= 0 时直接返回 false、&gt;= 1 时直接返回 true，
    /// 不进入随机数比较，这样调用方把概率设为 0 或 1 就能得到<b>确定</b>的行为，
    /// 可用于"本批次绝不出 null"这类需要稳定结果的场景。
    /// </para>
    /// </summary>
    public sealed class ProbabilityNullPolicy : ITangdaoNullPolicy
    {
        /// <summary>
        /// 按配置概率决定本次是否产出 null。
        /// </summary>
        /// <param name="property">可空类型属性</param>
        /// <param name="context">生成上下文</param>
        public bool ShouldBeNull(PropertyInfo property, TangdaoGeneratorContext context)
        {
            double probability = context.Options.NullableNullProbability;

            if (probability <= 0d)
            {
                return false;
            }

            if (probability >= 1d)
            {
                return true;
            }

            return context.NextDouble() < probability;
        }
    }
}
