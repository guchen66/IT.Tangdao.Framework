using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 程序集扫描契约：从程序集里圈出"参与批量造值"的类型。
    /// <para>
    /// 【为什么抽成接口】"怎么找实体"是纯策略：默认实现走标记接口，
    /// 若项目另有圈定规则（自定义特性、配置文件清单、模块目录约定等），
    /// 替换实现即可，配置层与执行层无需改动。
    /// </para>
    /// <para>
    /// 扫描结果应当是<b>去重且顺序稳定</b>的具体类集合，可直接交给执行层逐个造值。
    /// </para>
    /// </summary>
    public interface ITangdaoFakerScanner
    {
        /// <summary>
        /// 扫描指定程序集，返回命中扫描配置的类型集合。
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        /// <param name="options">扫描配置，为 null 时按库默认口径扫描</param>
        /// <returns>命中的类型集合（未命中时为空集合，不返回 null）</returns>
        IReadOnlyList<Type> Scan(Assembly assembly, TangdaoFakerScanOptions options);
    }
}
