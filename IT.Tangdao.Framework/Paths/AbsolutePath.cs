using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Paths
{
    public readonly struct AbsolutePath : IEquatable<AbsolutePath>, IComparable<AbsolutePath>
    {
        private readonly string _path;

        private AbsolutePath(bool _) => _path = string.Empty;

        public AbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            _path = Path.GetFullPath(path); // 确保是绝对路径
        }

        /// <summary>
        /// 空对象模式
        /// </summary>
        public static AbsolutePath Empty { get; } = new AbsolutePath(false);

        public string Value => _path;
        public bool Exists => File.Exists(_path) || Directory.Exists(_path);
        public bool FileExists => File.Exists(_path);
        public bool DirectoryExists => Directory.Exists(_path);

        /// <summary>
        /// 判断是否是根目录
        /// 根目录格式（如 "E:\\" 或 "/" 或"E:/"或"E://"）
        /// </summary>
        public bool IsRooted
        {
            get
            {
                string root = Path.GetPathRoot(_path);
                return _path.EqualsIgnoreCase(root);
            }
        }

        public override string ToString() => _path;

        public bool Equals(AbsolutePath other) =>
            string.Equals(_path, other._path, StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is AbsolutePath other && Equals(other);

        public override int GetHashCode() => _path.GetHashCode();

        public static bool operator ==(AbsolutePath left, AbsolutePath right) => left.Equals(right);

        public static bool operator !=(AbsolutePath left, AbsolutePath right) => !left.Equals(right);

        public static explicit operator string(AbsolutePath path) => path._path;

        /// <summary>
        /// 组合路径
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public AbsolutePath Combine(string relativePath)
        {
            string newPath = Path.Combine(_path, relativePath);
            return new AbsolutePath(newPath);
        }

        /// <summary>
        /// 对路径追加扩展名
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public AbsolutePath WithExtension(string extension)
        {
            string newPath = Path.ChangeExtension(_path, extension);
            return new AbsolutePath(newPath);
        }

        /// <summary>
        /// 返回路径的上一级目录
        /// </summary>
        /// <returns></returns>
        public AbsolutePath Parent()
        {
            string parent = Path.GetDirectoryName(_path);
            return new AbsolutePath(parent ?? _path);
        }

        /// <summary>
        /// 目录名称或文件名称
        /// </summary>
        public string FileName => Path.GetFileName(_path);

        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(_path);

        /// <summary>
        /// 转换为相对于解决方案的相对路径
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public RelativePath InSolution()
        {
            // 方法1：自动查找解决方案目录（推荐）
            string solutionDir = FindSolutionDirectory();

            if (solutionDir == null)
            {
                // 方法2：回退方案，使用调用者文件路径推断
                solutionDir = InferSolutionDirectoryFromSource();
            }

            if (solutionDir == null)
            {
                throw new InvalidOperationException($"无法从路径 '{_path}' 解析解决方案目录。请确保在解决方案中运行，或手动指定 solutionDirectory。");
            }

            return MakeRelativeTo(solutionDir);
        }

        // 重载：允许指定解决方案目录
        public RelativePath InSolution(string solutionDirectory)
        {
            var solutionPath = new AbsolutePath(solutionDirectory);
            return MakeRelativeTo(solutionPath.Value);
        }

        // 通用的相对路径计算方法
        public RelativePath MakeRelativeTo(string basePath)
        {
            var baseUri = new Uri(Path.GetFullPath(basePath) + Path.DirectorySeparatorChar);
            var pathUri = new Uri(_path);

            Uri relativeUri = baseUri.MakeRelativeUri(pathUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString())
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            return new RelativePath(relativePath);
        }

        public RelativePath MakeRelativeTo(AbsolutePath basePath) =>
            MakeRelativeTo(basePath.Value);

        /// <summary>
        /// depth < 8 防止过深或奇葩挂载；
        /// </summary>
        /// <returns></returns>
        private static string FindSolutionDirectory()
        {
            DirectoryInfo current = new DirectoryInfo(Directory.GetCurrentDirectory());
            for (int depth = 0; current != null && depth < 8; depth++) // 最多 8 级
            {
                try
                {
                    if (current.GetFiles("*.sln").Length != 0)
                        return current.FullName;
                }
                catch (UnauthorizedAccessException)
                {
                    // 跳过无权限目录
                }

                current = current.Parent;
            }
            return null;
        }

        private static string InferSolutionDirectoryFromSource([CallerFilePath] string sourceFilePath = null)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
                return null;

            // 假设源文件在 src/项目/文件.cs，解决方案在上级
            var sourceFile = new AbsolutePath(sourceFilePath);

            // 典型的项目结构：解决方案目录包含 src、test 等文件夹
            DirectoryInfo current = new DirectoryInfo(sourceFile.Parent().Value);

            for (int i = 0; i < 4 && current != null; i++) // 最多向上4级
            {
                if (current.GetFiles("*.sln").Length > 0)
                    return current.FullName;

                // 检查是否是典型的解决方案结构
                if (current.GetDirectories("src").Length > 0 ||
                    current.GetDirectories("test").Length > 0 ||
                    current.GetDirectories("tests").Length > 0)
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            return null;
        }

        public int CompareTo(AbsolutePath other)
        {
            return _path.CompareIgnoreCase(other.Value);
        }
    }
}