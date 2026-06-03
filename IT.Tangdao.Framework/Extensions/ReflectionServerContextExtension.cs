using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Common.Reflection;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 提供基于 <see cref="ReflectionServerContext"/> 的类型缓存扩展方法。
    /// 用于将扫描的程序集类型快速注册到全局缓存中，提升后续反射查询效率。
    /// </summary>
    public static class ReflectionServerContextExtension
    {
        /// <summary>
        /// 已加载的程序集集合，用于去重和全局共享。
        /// </summary>
        internal static HashSet<Assembly> assemblies = new HashSet<Assembly>();

        /// <summary>
        /// 线程安全的类型缓存字典，Key 为类型名称，Value 为 <see cref="Type"/> 实例。
        /// </summary>
        internal static ConcurrentDictionary<string, Type> reflectionTypes = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// 将 <paramref name="reflectionServerContext"/> 中的所有类型构建到全局缓存中。
        /// </summary>
        /// <param name="reflectionServerContext">反射服务上下文，包含待缓存的类型集合。</param>
        public static void Builder(this ReflectionServerContext reflectionServerContext)
        {
            foreach (var item in reflectionServerContext._types)
            {
                reflectionTypes.GetOrAdd(item.Name, item);
            }
        }
    }
}