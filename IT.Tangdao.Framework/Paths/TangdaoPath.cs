using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Paths
{
    public sealed class TangdaoPath
    {
        #region 单例

        public static TangdaoPath Instance { get; } = new TangdaoPath();

        private TangdaoPath()
        {
        }

        #endregion 单例

        #region 缓存

        /// <summary>
        /// 路径缓存，用于存储频繁访问的路径，提高性能
        /// 使用 StringComparer.OrdinalIgnoreCase 确保路径比较不区分大小写
        /// </summary>
        private static readonly ConcurrentDictionary<string, AbsolutePath> _cache = new ConcurrentDictionary<string, AbsolutePath>(StringComparer.OrdinalIgnoreCase);

        #endregion 缓存

        #region 核心工厂

        /// <summary>
        /// 当前源文件绝对路径（编译期注入）
        /// </summary>
        /// <param name="filePath">编译期注入的当前文件路径</param>
        /// <returns>当前文件的绝对路径</returns>
        public AbsolutePath GetThisFilePath([CallerFilePath] string filePath = null)
        {
            // 为每个文件路径创建唯一缓存键，避免不同文件共享同一缓存
            return _cache.GetOrAdd($"__file_{filePath}", _ => new AbsolutePath(filePath));
        }

        /// <summary>
        /// 解决方案根目录（带缓存）
        /// </summary>
        /// <returns>解决方案根目录的绝对路径</returns>
        public AbsolutePath GetSolutionDirectory() => _cache.GetOrAdd("__sln", _ => InternalGetSolutionDirectory());

        /// <summary>
        /// 内部获取解决方案目录的方法
        /// </summary>
        /// <returns>解决方案目录的绝对路径</returns>
        private AbsolutePath InternalGetSolutionDirectory()
        {
            var dir = FindSolutionDirectory();
            return new AbsolutePath(dir);
        }

        /// <summary>
        /// 当前工作目录（带缓存）
        /// </summary>
        /// <returns>当前工作目录的绝对路径</returns>
        public AbsolutePath GetCurrentDirectory()
            // 缓存当前工作目录，避免频繁调用 Directory.GetCurrentDirectory()
            => _cache.GetOrAdd("__current", _ => new AbsolutePath(Directory.GetCurrentDirectory()));

        /// <summary>
        /// 临时目录（带缓存）
        /// </summary>
        /// <returns>系统临时目录的绝对路径</returns>
        public AbsolutePath GetTempDirectory()
            // 缓存临时目录，避免频繁调用 Path.GetTempPath()
            => _cache.GetOrAdd("__temp", _ => new AbsolutePath(Path.GetTempPath()));

        /// <summary>
        /// 环境变量目录（可 CI）
        /// </summary>
        /// <param name="envKey">环境变量名称</param>
        /// <param name="fallback">当环境变量不存在时的回退路径</param>
        /// <returns>环境变量指定的目录或回退路径</returns>
        public AbsolutePath GetEnvironmentDirectory(string envKey, AbsolutePath? fallback = null)
        {
            var val = Environment.GetEnvironmentVariable(envKey);
            return !string.IsNullOrEmpty(val) ? new AbsolutePath(val) : fallback ?? GetCurrentDirectory();
        }

        #endregion 核心工厂

        #region Fluent Builder

        /// <summary>
        /// 创建从解决方案目录开始的路径构建器
        /// </summary>
        /// <returns>路径构建器实例</returns>
        public TangdaoPathBuilder Solution() => new TangdaoPathBuilder(GetSolutionDirectory());

        /// <summary>
        /// 创建从当前目录开始的路径构建器
        /// </summary>
        /// <returns>路径构建器实例</returns>
        public TangdaoPathBuilder Current() => new TangdaoPathBuilder(GetCurrentDirectory());

        /// <summary>
        /// 创建从临时目录开始的路径构建器
        /// </summary>
        /// <returns>路径构建器实例</returns>
        public TangdaoPathBuilder Temp() => new TangdaoPathBuilder(GetTempDirectory());

        /// <summary>
        /// 创建自定义根目录的路径构建器
        /// </summary>
        /// <param name="rootDir">自定义根目录路径</param>
        /// <returns>路径构建器实例</returns>
        public TangdaoPathBuilder AsPath(string rootDir = null) => new TangdaoPathBuilder(string.IsNullOrWhiteSpace(rootDir) ? GetCurrentDirectory() : new AbsolutePath(rootDir));

        #endregion Fluent Builder

        #region 私有辅助

        /// <summary>
        /// 查找解决方案目录
        /// </summary>
        /// <returns>解决方案目录的完整路径</returns>
        /// <exception cref="InvalidOperationException">当找不到解决方案目录时抛出</exception>
        private static string FindSolutionDirectory()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var slnFiles = dir.GetFiles("*.sln");
                if (slnFiles.Length > 0)
                {
                    // 取第一个 .sln 文件名（不含扩展名）
                    string slnName = Path.GetFileNameWithoutExtension(slnFiles[0].Name);
                    // 如果同级有同名文件夹，就使用该文件夹作为解决方案目录
                    var projectDir = Path.Combine(dir.FullName, slnName);
                    if (Directory.Exists(projectDir))
                        return projectDir;
                    // 否则使用 .sln 文件所在目录作为解决方案目录
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            // 直接抛出异常，避免在调用处再次检查
            throw new InvalidOperationException("未找到 .sln 目录。");
        }

        #endregion 私有辅助

        #region 日期路径功能

        /// <summary>
        /// 生成基于当前日期的目录路径（格式：基础路径/年/月/日）
        /// </summary>
        /// <param name="basePath">基础路径，默认为当前目录</param>
        /// <returns>基于日期的目录路径</returns>
        public AbsolutePath GetDateDirectory(string basePath = null)
        {
            // 使用当前日期和基础路径生成唯一缓存键
            var cacheKey = string.IsNullOrEmpty(basePath) ? "__date_dir" : $"__date_dir_{basePath}";

            return _cache.GetOrAdd(cacheKey, _ =>
            {
                var baseDir = string.IsNullOrEmpty(basePath)
                    ? GetCurrentDirectory()
                    : new AbsolutePath(basePath);

                DateTime now = DateTime.Now;
                return baseDir
                    .Combine(now.Year.ToString())
                    .Combine(now.Month.ToString("00"))
                    .Combine(now.Day.ToString("00"));
            });
        }

        /// <summary>
        /// 生成基于当前日期的文件路径（格式：基础路径/年/月/日/日期后缀.扩展名）
        /// </summary>
        /// <param name="basePath">基础路径，默认为当前目录</param>
        /// <param name="fileSuffix">文件名后缀</param>
        /// <param name="extension">文件扩展名，默认为 xlsx</param>
        /// <returns>基于日期的文件路径</returns>
        public AbsolutePath GetDateFilePath(string basePath = null, string fileSuffix = "", string extension = "xlsx")
        {
            // 使用当前日期、基础路径、文件后缀和扩展名生成唯一缓存键
            var cacheKey = string.IsNullOrEmpty(basePath)
                ? $"__date_file_{fileSuffix}_{extension}"
                : $"__date_file_{basePath}_{fileSuffix}_{extension}";

            return _cache.GetOrAdd(cacheKey, _ =>
            {
                var dateDir = GetDateDirectory(basePath);
                string date = DateTime.Now.ToString("yyMMdd");
                string fileName = string.IsNullOrEmpty(fileSuffix)
                    ? $"{date}.{extension}"
                    : $"{date}_{fileSuffix}.{extension}";

                return dateDir.Combine(fileName);
            });
        }

        /// <summary>
        /// Fluent API 版本 - 基于指定基础路径的日期目录构建器
        /// </summary>
        /// <param name="basePath">基础路径</param>
        /// <returns>日期路径构建器实例</returns>
        public TangdaoDatePathBuilder DateFrom(string basePath) => new TangdaoDatePathBuilder(new AbsolutePath(basePath));

        #endregion 日期路径功能
    }
}