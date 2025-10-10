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
        { }

        #endregion 单例

        #region 缓存

        private static readonly ConcurrentDictionary<string, AbsolutePath> _cache =
            new ConcurrentDictionary<string, AbsolutePath>(StringComparer.OrdinalIgnoreCase);

        #endregion 缓存

        #region 核心工厂

        /// <summary>
        /// 当前源文件绝对路径（编译期注入）
        /// </summary>
        public AbsolutePath GetThisFilePath([CallerFilePath] string filePath = null)
            => new AbsolutePath(filePath);

        /// <summary>
        /// 解决方案根目录（带缓存）
        /// </summary>
        public AbsolutePath GetSolutionDirectory() => _cache.GetOrAdd("__sln", _ => GetSolutionDirectoryCore());

        private AbsolutePath GetSolutionDirectoryCore()
        {
            var dir = FindSolutionDirectory();
            if (dir == null)
                throw new InvalidOperationException("未找到 .sln 目录。");
            return new AbsolutePath(dir);
        }

        /// <summary>
        /// 当前工作目录
        /// </summary>
        public AbsolutePath GetCurrentDirectory()
            => new AbsolutePath(Directory.GetCurrentDirectory());

        /// <summary>
        /// 临时目录
        /// </summary>
        public AbsolutePath GetTempDirectory()
            => new AbsolutePath(Path.GetTempPath());

        /// <summary>
        /// 环境变量目录（可 CI）
        /// </summary>
        public AbsolutePath GetEnvironmentDirectory(string envKey, AbsolutePath? fallback = null)
        {
            var val = Environment.GetEnvironmentVariable(envKey);
            return !string.IsNullOrEmpty(val) ? new AbsolutePath(val)
                                              : fallback ?? GetCurrentDirectory();
        }

        #endregion 核心工厂

        #region Fluent Builder

        public TangdaoPathBuilder Solution() => new TangdaoPathBuilder(GetSolutionDirectory());

        public TangdaoPathBuilder Current() => new TangdaoPathBuilder(GetCurrentDirectory());

        public TangdaoPathBuilder Temp() => new TangdaoPathBuilder(GetTempDirectory());

        #endregion Fluent Builder

        #region 私有辅助

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
                    // 如果同级有同名文件夹，就下去
                    var projectDir = Path.Combine(dir.FullName, slnName);
                    if (Directory.Exists(projectDir))
                        return projectDir;
                    // 否则停在 .sln 层
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            throw new InvalidOperationException("未找到 .sln 目录。");
        }

        #endregion 私有辅助
    }
}