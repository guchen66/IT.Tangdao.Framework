using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 非泛型批量造值契约：对"运行时才知道的类型"生成一批假数据。
    /// <para>
    /// 【为什么需要它】<c>TangdaoDataFaker&lt;T&gt;</c> 要求类型在编译期已知，
    /// 而按程序集批量生成时类型是反射得来的 <see cref="Type"/>，无法套用泛型门面。
    /// 本契约把门面的造值语义（规则链裁决 + 空值策略 + 串行回填自增主键）
    /// 以非泛型形式等价提供，造值本身仍完全复用库内既有引擎，不另起一套逻辑。
    /// </para>
    /// <para>
    /// 返回集合的<b>运行时类型为 <c>List&lt;T&gt;</c></b>（T 为入参 type），
    /// 因此调用方既可按 <see cref="IList"/> 遍历，也可强转为 <c>List&lt;T&gt;</c> 使用。
    /// </para>
    /// </summary>
    public interface ITangdaoFakePopulator
    {
        /// <summary>
        /// 为指定类型生成一批随机实例。
        /// </summary>
        /// <param name="type">目标类型，需具备公共无参构造函数</param>
        /// <param name="count">生成数量，非正数时返回空集合</param>
        /// <param name="options">生成配置，为 null 时按库默认配置</param>
        /// <param name="registry">生成器注册表，为 null 时按库默认注册表</param>
        /// <param name="nullPolicy">可空属性的空值策略，为 null 时按库默认策略</param>
        /// <returns>长度为 count 的 <c>List&lt;type&gt;</c>（装箱为 <see cref="IList"/>）</returns>
        IList Populate(Type type, int count, TangdaoFakerOptions options, ITangdaoGeneratorRegistry registry, ITangdaoNullPolicy nullPolicy);
    }
}
