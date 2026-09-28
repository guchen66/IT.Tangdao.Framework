using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 类型生成器与命名模板的注册表。
    /// <para>
    /// 本接口承载了库对外的<b>两个扩展点</b>：
    /// </para>
    /// <para>
    /// ① 注册自定义 <see cref="ITangdaoValueGenerator"/> —— 扩展"某种类型该怎么造值"，
    /// 例如让库支持 <c>IPAddress</c>、<c>Uri</c> 等业务类型；
    /// </para>
    /// <para>
    /// ② 注册自定义命名模板 —— 扩展 <c>[TangdaoFake(Template = "...")]</c> 可用的数据池，
    /// 例如自建"产品编号""部门名称"等词库。
    /// </para>
    /// <para>
    /// 注册表实例可通过 <c>TangdaoDataFaker&lt;T&gt;.Registry</c> 替换，
    /// 因此调用方可以在不污染全局默认表的前提下定制生成行为。
    /// </para>
    /// </summary>
    public interface ITangdaoGeneratorRegistry
    {
        /// <summary>
        /// 注册一个类型生成器。同一类型重复注册时，后注册的覆盖先注册的。
        /// </summary>
        /// <param name="generator">生成器实例</param>
        void Register(ITangdaoValueGenerator generator);

        /// <summary>
        /// 注册一个命名模板。
        /// </summary>
        /// <param name="template">模板名，建议直接使用 <c>MockTemplate</c> 中的常量</param>
        /// <param name="factory">模板工厂：接受生成上下文，返回该模板的一个数据</param>
        void RegisterTemplate(String template, Func<TangdaoGeneratorContext, object> factory);

        /// <summary>
        /// 按类型查找生成器。
        /// </summary>
        /// <param name="type">目标类型（调用方已做可空解包）</param>
        /// <param name="generator">命中的生成器；未命中时为 null</param>
        /// <returns>是否命中</returns>
        bool TryResolve(Type type, out ITangdaoValueGenerator generator);

        /// <summary>
        /// 按模板名查找模板工厂。
        /// </summary>
        /// <param name="template">模板名</param>
        /// <param name="factory">命中的工厂；未命中时为 null</param>
        /// <returns>是否命中</returns>
        bool TryResolveTemplate(String template, out Func<TangdaoGeneratorContext, object> factory);
    }
}
