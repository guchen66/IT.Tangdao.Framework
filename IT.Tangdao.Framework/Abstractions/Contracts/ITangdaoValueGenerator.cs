using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 类型值生成契约：定义"某一类类型该怎么造值"。
    /// <para>
    /// 每个生成器只负责一族类型（如全部整数类型共用 <c>IntegerValueGenerator</c>），
    /// 职责单一、便于替换与测试。
    /// </para>
    /// <para>
    /// 设计意图：把原先散落在 <c>TangdaoDataFaker</c> 里的 if-else 分支与硬编码字典，
    /// 收敛为"一个类型族一个类"，新增类型时只需实现本接口并注册，不必改动库源码。
    /// </para>
    /// <para>
    /// 扩展方式：实现本接口后通过 <see cref="ITangdaoGeneratorRegistry.Register"/> 注册即可。
    /// </para>
    /// </summary>
    public interface ITangdaoValueGenerator
    {
        /// <summary>
        /// 判断本生成器是否负责该类型。
        /// </summary>
        /// <param name="type">待判断的目标类型。注意：调用方已完成可空解包，此处不会收到 <c>Nullable&lt;T&gt;</c></param>
        /// <returns>true 表示由本生成器生成</returns>
        bool CanGenerate(Type type);

        /// <summary>
        /// 为指定类型生成一个值。
        /// </summary>
        /// <param name="type">目标类型。同样为已解包类型</param>
        /// <param name="context">生成上下文，提供随机源、配置与注册表</param>
        /// <returns>生成结果。返回值必须与 <paramref name="type"/> 兼容，否则赋值时会抛异常</returns>
        object Generate(Type type, TangdaoGeneratorContext context);
    }
}
