using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// Assembly帮助类
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// 拿到 dll 所在文件夹（兼容单文件发布）
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetDirectory(this Assembly asm)
            => Path.GetDirectoryName(asm.Location) ?? AppContext.BaseDirectory;   // 单文件发布时 Location 为空

        /// <summary>
        /// 获取所有公开“可注入”类型（非抽象类+公共构造）
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetInjectableTypes(this Assembly asm)
            => asm.GetTypes()
                  .Where(t => t.IsClass && !t.IsAbstract && t.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length > 0)
                  .ToArray();

        /// <summary>
        /// 按名字模糊找类型（忽略大小写、命名空间）
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public static Type FindType(this Assembly asm, string shortName)
            => asm.GetTypes()
                  .FirstOrDefault(t => t.Name.Equals(shortName, StringComparison.OrdinalIgnoreCase)
                                    || t.FullName.Equals(shortName, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// 拿到标记了指定 Attribute 的所有类型
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="asm"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetMarkedTypes<TAttr>(this Assembly asm, bool inherit = false)
            where TAttr : Attribute
            => asm.GetTypes().Where(t => t.GetCustomAttributes(typeof(TAttr), inherit).Length > 0);

        /// <summary>
        /// 拿到标记了指定 Attribute 的所有方法
        /// </summary>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="asm"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetMarkedMethods<TAttr>(this Assembly asm, bool inherit = false)
            where TAttr : Attribute
            => asm.GetTypes()
                  .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                  .Where(m => m.GetCustomAttributes(typeof(TAttr), inherit).Length > 0);

        /// <summary>
        /// 快速实例化——无参构造
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static object CreateInstance(this Assembly asm, string typeName)
        {
            var t = asm.FindType(typeName);
            return t == null
                ? throw new InvalidOperationException($"Type {typeName} not found")
                : Activator.CreateInstance(t);
        }

        /// <summary>
        /// 拿到嵌入式资源流（无异常版）
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Stream GetResourceStream(this Assembly asm, string name)
            => asm.GetManifestResourceStream(asm.GetManifestResourceNames()
                                                .FirstOrDefault(n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase)));

        /// <summary>
        /// 把嵌入式资源一次性读成字符串
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetResourceString(this Assembly asm, string name)
        {
            using (var s = asm.GetResourceStream(name))
            {
                if (s == null) return null;
                var sr = new StreamReader(s);
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 获取文件版本（AssemblyInformationalVersion 优先）
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetProductVersion(this Assembly asm)
            => asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
               ?? asm.GetName().Version?.ToString()
               ?? "0.0.0";

        /// <summary>
        /// 判断当前 dll 是否为 Debug 编译
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static bool IsDebugBuild(this Assembly asm)
        {
            var debugAttr = asm.GetCustomAttribute<DebuggableAttribute>();
            return debugAttr != null && debugAttr.IsJITTrackingEnabled;
        }

        /// <summary>
        /// 扫描所有引用，返回“真正”被用到的（跳过系统）
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static IReadOnlyList<AssemblyName> GetUserReferences(this Assembly asm)
            => asm.GetReferencedAssemblies()
                  .Where(a => !a.Name.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                              && !a.Name.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
                              && !a.Name.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase))
                  .ToArray();

        /// <summary>
        /// 返回要搜索的程序集列表；子类可重写过滤
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Assembly> GetModuleAssemblies()
        {
            // 1. 立即求值，避免多次枚举
            var loaded = AppDomain.CurrentDomain.GetAssemblies()
                                  .Where(a => !a.IsDynamic);

            var set = new HashSet<Assembly>(/* 默认引用相等 */);

            // 2. 入口程序集（可能 null）
            var entry = Assembly.GetEntryAssembly();
            if (entry != null)
                set.Add(entry);

            foreach (var a in loaded)
                if (IsFrameworkCandidate(a))
                    set.Add(a);

            return set;

            // 本地函数：过滤规则
            bool IsFrameworkCandidate(Assembly a)
            {
                var name = a.FullName;
                return
                    !name.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
                    !name.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) &&
                    !name.StartsWith("Presentation", StringComparison.OrdinalIgnoreCase) &&
                    !name.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase) &&
                    name.StartsWith("IT.Tangdao", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}