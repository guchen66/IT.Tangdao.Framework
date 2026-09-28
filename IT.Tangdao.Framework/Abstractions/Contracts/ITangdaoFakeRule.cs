using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 属性级生成规则契约：决定"这个属性该由谁来负责、生成什么值"。
    /// <para>
    /// 多个规则按序组成责任链（<c>FakeRuleChain</c>），<b>优先级由链中顺序决定</b>：
    /// 越靠前优先级越高，第一个 <see cref="CanHandle"/> 返回 true 的规则接手，
    /// 后续规则不再参与。
    /// </para>
    /// <para>
    /// 当前默认链的优先级：
    /// 显式声明值 &gt; 字符串长度 &gt; 数值范围 &gt; 自增主键 &gt; 命名模板 &gt; 按类型兜底。
    /// </para>
    /// <para>
    /// 设计意图：把原先 <c>GenerateRandomValue</c> 里的一长串 if-else 拆成可插拔的规则对象，
    /// 调整优先级或插入新规则时不需要改动既有代码。
    /// </para>
    /// </summary>
    public interface ITangdaoFakeRule
    {
        /// <summary>
        /// 判断本规则是否接手该属性。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <returns>true 表示由本规则生成该属性的值</returns>
        bool CanHandle(PropertyInfo property);

        /// <summary>
        /// 生成该属性的值。
        /// </summary>
        /// <param name="property">待处理的属性</param>
        /// <param name="context">生成上下文，提供随机源、配置与注册表</param>
        /// <returns>
        /// 属性值。值类型属性必须返回非 null（否则赋值会抛异常）；
        /// 引用类型属性返回 null 表示"本次不赋有效值"。
        /// </returns>
        object Resolve(PropertyInfo property, TangdaoGeneratorContext context);
    }
}
