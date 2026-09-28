using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 假数据生成的全局配置。
    /// <para>
    /// 提供"库级默认策略 + 调用方按需覆盖"两层控制：
    /// 库内置一份 <see cref="Default"/> 共享实例，所有未显式指定配置的调用方共用它；
    /// 若某个调用方需要独立配置（例如让本批次数据不出 null），
    /// 自行 <c>new TangdaoFakerOptions()</c> 后赋给 <c>TangdaoDataFaker&lt;T&gt;.Options</c> 即可，
    /// 不会影响其它调用方。
    /// </para>
    /// </summary>
    public sealed class TangdaoFakerOptions
    {
        /// <summary>全局默认配置实例。</summary>
        private static readonly TangdaoFakerOptions _default = new TangdaoFakerOptions();

        /// <summary>
        /// 全局默认配置。
        /// <para>警告：直接修改本实例的属性会影响所有未显式指定配置的调用方，请谨慎。</para>
        /// </summary>
        public static TangdaoFakerOptions Default
        {
            get { return _default; }
        }

        /// <summary>
        /// 可空类型（<c>T?</c>）产出 null 的概率，取值区间 [0,1]。
        /// <para>0 表示永不出 null（即把 T? 当 T 用）；1 表示恒为 null；默认 0.2。</para>
        /// </summary>
        public double NullableNullProbability { get; set; }

        /// <summary>
        /// <c>Build</c> 是否启用并行生成。默认 true。
        /// <para>关闭后改为普通 for 循环，便于调试时逐条跟踪。</para>
        /// </summary>
        public bool EnableParallel { get; set; }

        /// <summary>
        /// 嵌套对象的最大递归深度，超过该深度不再继续向下填充。默认 2。
        /// <para>用于防止深层对象图带来的性能损耗。</para>
        /// </summary>
        public int MaxNestedDepth { get; set; }

        /// <summary>日期类数据生成的默认下界。默认 1990-01-01。</summary>
        public DateTime MinDate { get; set; }

        /// <summary>日期类数据生成的默认上界。默认当天。</summary>
        public DateTime MaxDate { get; set; }

        /// <summary>
        /// 构造一份使用库默认值的配置。
        /// </summary>
        public TangdaoFakerOptions()
        {
            NullableNullProbability = 0.2;
            EnableParallel = true;
            MaxNestedDepth = 2;
            MinDate = new DateTime(1990, 1, 1);
            MaxDate = DateTime.Today;
        }
    }
}
