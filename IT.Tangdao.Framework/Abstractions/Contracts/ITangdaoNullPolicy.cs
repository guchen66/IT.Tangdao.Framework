using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 可空类型（<c>T?</c>）的空值语义策略。
    /// <para>
    /// 语义约定：用户写 <c>T?</c> 表示"既允许 null 也允许有效值"，
    /// 因此库的默认行为应当是<b>随机产出 null 或正确值</b>，而不是一律给 null、
    /// 也不是一律给有效值。本策略负责裁决"这一次该不该给 null"。
    /// </para>
    /// <para>
    /// 默认实现为按概率裁决（<c>ProbabilityNullPolicy</c>，概率取自
    /// <see cref="TangdaoFakerOptions.NullableNullProbability"/>）；
    /// 如需"恒不出 null""恒出 null"等其它语义，替换实现即可。
    /// </para>
    /// <para>
    /// 注意：本策略<b>只对可空类型生效</b>，非可空属性不会被询问。
    /// </para>
    /// </summary>
    public interface ITangdaoNullPolicy
    {
        /// <summary>
        /// 判断本次是否应该产出 null。
        /// </summary>
        /// <param name="property">可空类型属性</param>
        /// <param name="context">生成上下文，提供配置与随机源</param>
        /// <returns>true 表示本次产出 null</returns>
        bool ShouldBeNull(PropertyInfo property, TangdaoGeneratorContext context);
    }
}
