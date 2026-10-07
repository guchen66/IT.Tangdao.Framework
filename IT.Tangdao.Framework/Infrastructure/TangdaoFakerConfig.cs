using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Common.Faker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 批量假数据生成配置（对标 Mapster 的 <c>TypeAdapterConfig</c>）。
    /// <para>
    /// 【典型用法】
    /// <code>
    /// TangdaoFakerConfig config = new TangdaoFakerConfig();
    /// config.Scan(typeof(Program).Assembly);              // 圈定实体
    /// Dictionary&lt;Type, object&gt; all = config.Build(5);    // 每类各造 5 条（value 为 List&lt;T&gt;）
    /// List&lt;User&gt; users = config.BuildFor&lt;User&gt;(10);        // 强类型取回
    /// </code>
    /// </para>
    /// <para>
    /// 【为什么单独持有 Registry / NullPolicy / Options】
    /// 泛型门面 <c>TangdaoDataFaker&lt;T&gt;</c> 的这三项是<b>按封闭类型各存一份</b>的静态状态，
    /// 批量场景下类型是运行时枚举出来的，既无从逐类型设置，也不该反过来污染门面。
    /// 因此本配置对象持有自己的实例（默认取库内默认值），配置之间互不影响。
    /// </para>
    /// <para>
    /// 【四个可替换的扩展点】<see cref="Scanner"/>、<see cref="Populator"/>、
    /// <see cref="Registry"/> / <see cref="NullPolicy"/> / <see cref="Options"/>，以及
    /// <see cref="ScanOptions"/> 上的排除集合与附加谓词；传入 null 时自动回落到库默认实现。
    /// </para>
    /// </summary>
    public class TangdaoFakerConfig
    {
        /// <summary>
        /// 一次 <c>Scan</c> 调用登记的内容：目标程序集 + 本次的通道参数。
        /// <para>
        /// 通道参数按次记录而不是覆盖全局，是为了让
        /// <c>Scan(asm)</c> 与 <c>Scan(asm, "MyApp.Entities")</c> 能在同一份配置里共存。
        /// </para>
        /// </summary>
        private sealed class AssemblyScanRequest
        {
            /// <summary>目标程序集。</summary>
            public Assembly Assembly;

            /// <summary>本次使用的标记类型；为 null 表示沿用 <see cref="TangdaoFakerScanOptions.MarkerType"/>。</summary>
            public Type MarkerType;

            /// <summary>本次使用的命名空间前缀；为 null 表示不开启命名空间补充通道。</summary>
            public string NamespacePrefix;
        }

        /// <summary>已登记的扫描请求，按登记顺序生效。</summary>
        private readonly List<AssemblyScanRequest> _requests = new List<AssemblyScanRequest>();

        private ITangdaoFakerScanner _scanner = new TangdaoFakerScanner();
        private ITangdaoFakePopulator _populator = new TangdaoFakePopulator();
        private ITangdaoGeneratorRegistry _registry = FakeDefaults.Registry;
        private ITangdaoNullPolicy _nullPolicy = FakeDefaults.NullPolicy;
        private TangdaoFakerOptions _options = TangdaoFakerOptions.Default;

        /// <summary>
        /// 扫描器。替换后后续 <see cref="ResolveTypes"/> / <see cref="Build"/> 立即生效；传入 null 则恢复默认。
        /// </summary>
        public ITangdaoFakerScanner Scanner
        {
            get { return _scanner; }
            set { _scanner = value ?? new TangdaoFakerScanner(); }
        }

        /// <summary>
        /// 造值器。替换后后续 <see cref="Build"/> / <see cref="BuildFor{T}"/> 立即生效；传入 null 则恢复默认。
        /// </summary>
        public ITangdaoFakePopulator Populator
        {
            get { return _populator; }
            set { _populator = value ?? new TangdaoFakePopulator(); }
        }

        /// <summary>
        /// 生成器注册表。替换后立即生效；传入 null 则恢复默认。
        /// </summary>
        public ITangdaoGeneratorRegistry Registry
        {
            get { return _registry; }
            set { _registry = value ?? FakeDefaults.Registry; }
        }

        /// <summary>
        /// 可空属性的空值策略。替换后立即生效；传入 null 则恢复默认。
        /// </summary>
        public ITangdaoNullPolicy NullPolicy
        {
            get { return _nullPolicy; }
            set { _nullPolicy = value ?? FakeDefaults.NullPolicy; }
        }

        /// <summary>
        /// 生成配置。替换后立即生效；传入 null 则恢复默认。
        /// </summary>
        public TangdaoFakerOptions Options
        {
            get { return _options; }
            set { _options = value ?? TangdaoFakerOptions.Default; }
        }

        /// <summary>
        /// 扫描配置（标记类型、命名空间通道开关、排除集合、附加谓词）。
        /// <para>本实例随配置创建，属性不暴露 setter，避免把整份扫描配置替换成共享实例。</para>
        /// </summary>
        public TangdaoFakerScanOptions ScanOptions { get; private set; }

        /// <summary>
        /// 构造一份使用库默认组件的配置。
        /// </summary>
        public TangdaoFakerConfig()
        {
            ScanOptions = new TangdaoFakerScanOptions();
        }

        /// <summary>
        /// 扫描程序集，圈定"实现了标记接口"的类型（主通道）。
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        /// <returns>本配置，便于链式调用</returns>
        public TangdaoFakerConfig Scan(Assembly assembly)
        {
            return AddScanRequest(assembly, null, null);
        }

        /// <summary>
        /// 扫描程序集，并<b>额外</b>开启命名空间前缀补充通道（用于尚未实现标记接口的老实体）。
        /// <para>两条通道取并集：除落在前缀内的类型外，实现了标记接口的类型同样会被纳入。</para>
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        /// <param name="namespacePrefix">命名空间前缀，如 <c>MyApp.Entities</c></param>
        /// <returns>本配置，便于链式调用</returns>
        public TangdaoFakerConfig Scan(Assembly assembly, string namespacePrefix)
        {
            if (string.IsNullOrEmpty(namespacePrefix))
            {
                throw new ArgumentException("命名空间前缀不能为空", nameof(namespacePrefix));
            }

            return AddScanRequest(assembly, null, namespacePrefix);
        }

        /// <summary>
        /// 扫描程序集，圈定"实现了自定义标记接口"的类型。
        /// <para>
        /// 标记类型只要求是类（接口/抽象类皆可），实际判定用 <c>IsAssignableFrom</c>，
        /// 因此不能约束 <c>new()</c> —— 接口无法被实例化。
        /// </para>
        /// </summary>
        /// <typeparam name="TMarker">自定义标记类型</typeparam>
        /// <param name="assembly">目标程序集</param>
        /// <returns>本配置，便于链式调用</returns>
        public TangdaoFakerConfig Scan<TMarker>(Assembly assembly) where TMarker : class
        {
            return AddScanRequest(assembly, typeof(TMarker), null);
        }

        /// <summary>
        /// 解析当前登记的所有扫描请求，返回去重后的实体类型集合。
        /// <para>供调用方在造值前确认"圈到了哪些类型"，也便于排查标记未生效的问题。</para>
        /// </summary>
        /// <returns>实体类型集合（无命中时为空集合）</returns>
        public IReadOnlyList<Type> ResolveTypes()
        {
            List<Type> result = new List<Type>();
            HashSet<Type> seen = new HashSet<Type>();

            for (int i = 0; i < _requests.Count; i++)
            {
                AssemblyScanRequest request = _requests[i];
                IReadOnlyList<Type> types = Scanner.Scan(request.Assembly, BuildScanOptions(request));

                if (types == null)
                {
                    continue;
                }

                for (int j = 0; j < types.Count; j++)
                {
                    Type type = types[j];
                    if (type != null && seen.Add(type))
                    {
                        result.Add(type);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 非泛型批量生成：为扫描到的每个类型各造 <paramref name="count"/> 条数据。
        /// </summary>
        /// <param name="count">每个类型的生成数量，非正数时各项为空集合</param>
        /// <returns>键为实体类型，值为 <c>List&lt;该类型&gt;</c>（以 <see cref="object"/> 承载）</returns>
        public Dictionary<Type, object> Build(int count)
        {
            IReadOnlyList<Type> types = ResolveTypes();
            Dictionary<Type, object> result = new Dictionary<Type, object>();

            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                result[type] = Populator.Populate(type, count, Options, Registry, NullPolicy);
            }

            return result;
        }

        /// <summary>
        /// 强类型批量生成：语义等价于从 <see cref="Build"/> 结果中取 <typeparamref name="T"/> 对应的集合。
        /// <para>
        /// 与 <see cref="Build"/> 的区别只在取回形式：本方法直接得到 <c>List&lt;T&gt;</c>，无需再装箱转换。
        /// 即使 <typeparamref name="T"/> 未出现在扫描结果中（例如临时只想造某一种实体），
        /// 也会按显式指定的类型直接生成，便于单类型场景复用同一份配置。
        /// </para>
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="count">生成数量，非正数时返回空集合</param>
        /// <returns>生成的强类型集合</returns>
        public List<T> BuildFor<T>(int count) where T : class
        {
            IList source = Populator.Populate(typeof(T), count, Options, Registry, NullPolicy);
            List<T> result = new List<T>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                result.Add((T)source[i]);
            }

            return result;
        }

        /// <summary>
        /// 登记一次扫描请求。
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        /// <param name="markerType">本次使用的标记类型，可为 null</param>
        /// <param name="namespacePrefix">本次使用的命名空间前缀，可为 null</param>
        /// <returns>本配置，便于链式调用</returns>
        private TangdaoFakerConfig AddScanRequest(Assembly assembly, Type markerType, string namespacePrefix)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            AssemblyScanRequest request = new AssemblyScanRequest
            {
                Assembly = assembly,
                MarkerType = markerType,
                NamespacePrefix = namespacePrefix
            };

            _requests.Add(request);
            return this;
        }

        /// <summary>
        /// 基于全局扫描配置复制一份，并按本次请求覆盖通道参数。
        /// <para>
        /// 每次扫描都用副本，既保证了排除集合与附加谓词对全部扫描请求一致生效，
        /// 又避免某次 <c>Scan</c> 的前缀设置泄漏到其它请求上。
        /// </para>
        /// </summary>
        /// <param name="request">扫描请求</param>
        private TangdaoFakerScanOptions BuildScanOptions(AssemblyScanRequest request)
        {
            TangdaoFakerScanOptions options = ScanOptions.Clone();

            if (request.MarkerType != null)
            {
                options.MarkerType = request.MarkerType;
            }

            if (!string.IsNullOrEmpty(request.NamespacePrefix))
            {
                options.EnableNamespaceScan = true;
                options.NamespacePrefix = request.NamespacePrefix;
            }

            return options;
        }
    }
}
