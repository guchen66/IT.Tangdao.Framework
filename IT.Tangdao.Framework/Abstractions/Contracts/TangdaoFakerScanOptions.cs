using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 批量扫描的配置项：决定"哪些类型算作实体"。
    /// <para>
    /// 【两条通道】
    /// </para>
    /// <list type="number">
    /// <item><description>主通道（默认开启）：<see cref="MarkerType"/> 的实现者，走 <c>IsAssignableFrom</c>，
    /// 天然覆盖继承链上的具体子类，是推荐用法；</description></item>
    /// <item><description>补充通道（默认关闭）：按命名空间前缀圈定类型，用于尚未实现标记接口的老实体，
    /// 需显式开启 <see cref="EnableNamespaceScan"/> 并给出 <see cref="NamespacePrefix"/>。</description></item>
    /// </list>
    /// <para>
    /// 【候选类型谓词】两条通道的候选都必须同时满足：
    /// IsClass &amp;&amp; !IsAbstract &amp;&amp; !ContainsGenericParameters(排除开放泛型)
    /// &amp;&amp; 存在公共无参构造 &amp;&amp; 未被 <c>TangdaoFakeIgnoreAttribute</c> 标注
    /// &amp;&amp; 不在 <see cref="ExcludeTypes"/> 中 &amp;&amp; 通过 <see cref="IncludePredicate"/>。
    /// </para>
    /// </summary>
    public sealed class TangdaoFakerScanOptions
    {
        /// <summary>全局默认配置实例。</summary>
        private static readonly TangdaoFakerScanOptions _default = new TangdaoFakerScanOptions();

        /// <summary>
        /// 全局默认扫描配置。
        /// <para>警告：直接修改本实例的属性会影响所有未显式指定扫描配置的调用方，请谨慎。</para>
        /// </summary>
        public static TangdaoFakerScanOptions Default
        {
            get { return _default; }
        }

        /// <summary>
        /// 主通道标记类型，默认 <c>typeof(ITangdaoFakeEntity)</c>。
        /// <para>改成本项目自定义的实体标记接口即可圈定另一批类型，无需改动扫描器实现。</para>
        /// </summary>
        public Type MarkerType { get; set; }

        /// <summary>
        /// 是否启用命名空间前缀补充通道。默认 false（关闭）。
        /// </summary>
        public bool EnableNamespaceScan { get; set; }

        /// <summary>
        /// 命名空间前缀，仅在 <see cref="EnableNamespaceScan"/> 为 true 时参与判定。
        /// <para>判定方式为前缀匹配（<c>StartsWith</c>），因此 <c>"MyApp.Entities"</c>
        /// 也会命中 <c>MyApp.Entities.Sub</c>。</para>
        /// </summary>
        public string NamespacePrefix { get; set; }

        /// <summary>
        /// 排除类型集合：命中的类型不参与批量生成，优先于两条通道的命中结果。
        /// <para>集合实例随本配置创建，可直接 <c>options.ExcludeTypes.Add(typeof(X))</c> 追加。</para>
        /// </summary>
        public ISet<Type> ExcludeTypes { get; private set; }

        /// <summary>
        /// 附加过滤谓词：返回 true 才纳入，返回 false 直接排除；默认 null（不附加约束）。
        /// <para>在候选谓词与两条通道判定之外的最后一道闸门，供调用方做业务性剔除。</para>
        /// </summary>
        public Func<Type, bool> IncludePredicate { get; set; }

        /// <summary>
        /// 构造一份使用库默认值的扫描配置。
        /// </summary>
        public TangdaoFakerScanOptions()
        {
            MarkerType = typeof(ITangdaoFakeEntity);
            EnableNamespaceScan = false;
            NamespacePrefix = null;
            ExcludeTypes = new HashSet<Type>();
            IncludePredicate = null;
        }

        /// <summary>
        /// 复制一份当前配置。
        /// <para>
        /// 供配置层"共享全局过滤条件 + 按次覆盖通道参数"使用：
        /// 每次扫描都基于同一份基准配置复制，避免单次 <c>Scan</c> 的通道参数污染后续扫描。
        /// </para>
        /// </summary>
        public TangdaoFakerScanOptions Clone()
        {
            TangdaoFakerScanOptions clone = new TangdaoFakerScanOptions
            {
                MarkerType = MarkerType,
                EnableNamespaceScan = EnableNamespaceScan,
                NamespacePrefix = NamespacePrefix,
                IncludePredicate = IncludePredicate
            };

            foreach (Type type in ExcludeTypes)
            {
                clone.ExcludeTypes.Add(type);
            }

            return clone;
        }
    }
}
