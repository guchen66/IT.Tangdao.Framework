using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common.Faker
{
    /// <summary>
    /// 默认扫描器：标记接口主通道 + 命名空间前缀补充通道。
    /// <para>
    /// 【底座复用】候选集合取自 <c>AssemblyExtension.GetInjectableTypes()</c>
    /// （已含 IsClass &amp;&amp; !IsAbstract &amp;&amp; 存在公共实例构造），
    /// 再补上它的两处缺口：开放泛型判定（<c>ContainsGenericParameters</c>）
    /// 与"公共<b>无参</b>构造"判定——批量造值必须能实例化，只要求"存在某个公共构造"不够用。
    /// </para>
    /// <para>
    /// 【两条通道的关系】取并集：主通道命中的类型与命名空间通道命中的类型都参与批量生成，
    /// 因此旧实体实现标记接口时可逐步迁移，不必一次改完。
    /// </para>
    /// </summary>
    public class TangdaoFakerScanner : ITangdaoFakerScanner
    {
        /// <summary>
        /// 扫描指定程序集，返回命中扫描配置的类型集合。
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        /// <param name="options">扫描配置，为 null 时按库默认口径扫描</param>
        /// <returns>命中的类型集合，顺序与 <c>Assembly.GetTypes()</c> 一致</returns>
        public IReadOnlyList<Type> Scan(Assembly assembly, TangdaoFakerScanOptions options)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            TangdaoFakerScanOptions scanOptions = options ?? new TangdaoFakerScanOptions();
            List<Type> matched = new List<Type>();

            foreach (Type type in assembly.GetInjectableTypes().Where(IsInstantiable))
            {
                if (!IsCandidate(type, scanOptions))
                {
                    continue;
                }

                if (IsMarkerMatch(type, scanOptions.MarkerType) || IsNamespaceMatch(type, scanOptions))
                {
                    matched.Add(type);
                }
            }

            return matched;
        }

        /// <summary>
        /// 前置快速过滤：开放泛型与无公共无参构造的类型直接出局。
        /// <para>
        /// 这两条与后面的 <see cref="IsCandidate"/> 不重复：开放泛型在表达式树实例化时必然抛异常，
        /// 提前剔除可以避免把明显不可造值的类型带进后续判定。
        /// </para>
        /// </summary>
        /// <param name="type">候选类型</param>
        private static bool IsInstantiable(Type type)
        {
            return !type.ContainsGenericParameters && HasPublicParameterlessConstructor(type);
        }

        /// <summary>
        /// 候选类型谓词：不满足者一律不参与批量生成（两条通道共用）。
        /// </summary>
        /// <param name="type">候选类型</param>
        /// <param name="options">扫描配置</param>
        private static bool IsCandidate(Type type, TangdaoFakerScanOptions options)
        {
            if (type == null || !type.IsClass || type.IsAbstract)
            {
                return false;
            }

            if (type.ContainsGenericParameters || !HasPublicParameterlessConstructor(type))
            {
                return false;
            }

            // inherit 显式传 true：与特性的 AttributeUsage(Inherited = true) 语义保持一致，
            // 即"排除标记"同样沿继承链传递
            if (Attribute.IsDefined(type, typeof(TangdaoFakeIgnoreAttribute), true))
            {
                return false;
            }

            if (options.ExcludeTypes != null && options.ExcludeTypes.Contains(type))
            {
                return false;
            }

            if (options.IncludePredicate != null && !options.IncludePredicate(type))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 主通道：类型是否实现了标记接口（含继承链上的实现）。
        /// </summary>
        /// <param name="type">候选类型</param>
        /// <param name="markerType">标记类型</param>
        private static bool IsMarkerMatch(Type type, Type markerType)
        {
            return markerType != null && markerType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 补充通道：类型是否落在指定命名空间前缀内（未开启时恒为 false）。
        /// </summary>
        /// <param name="type">候选类型</param>
        /// <param name="options">扫描配置</param>
        private static bool IsNamespaceMatch(Type type, TangdaoFakerScanOptions options)
        {
            if (!options.EnableNamespaceScan || string.IsNullOrEmpty(options.NamespacePrefix))
            {
                return false;
            }

            string typeNamespace = type.Namespace;
            if (typeNamespace == null)
            {
                return false;
            }

            return typeNamespace.StartsWith(options.NamespacePrefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// 是否具备公共无参构造函数（批量造值的实例化前提）。
        /// </summary>
        /// <param name="type">候选类型</param>
        private static bool HasPublicParameterlessConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
