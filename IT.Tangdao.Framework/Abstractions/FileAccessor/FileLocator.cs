using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Results;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Extensions;
using System.Xml.Linq;
using IT.Tangdao.Framework.Paths;
using System.Management;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public sealed class FileLocator : IFileLocator
    {
        /// <inheritdoc/>
        public IEnumerable<string> FindFiles(string directoryPath, string searchPattern, bool searchSubdirectories)
        {
            return InternalFindFiles(directoryPath, searchPattern, searchSubdirectories);
        }

        /// <inheritdoc/>
        public IEnumerable<string> FindFiles(AbsolutePath absolutePath, string searchPattern, bool searchSubdirectories)
        {
            return InternalFindFiles(absolutePath.Value, searchPattern, searchSubdirectories);
        }

        /// <inheritdoc/>
        public string FindFirst(string directoryPath, string searchPattern, bool searchSubdirectories)
        {
            return FindFiles(directoryPath, searchPattern, searchSubdirectories).FirstOrDefault();
        }

        /// <inheritdoc/>
        public string FindFirst(AbsolutePath absolutePath, string searchPattern, bool searchSubdirectories)
        {
            return FindFiles(absolutePath.Value, searchPattern, searchSubdirectories).FirstOrDefault();
        }

        /// <summary>
        /// 共享核心逻辑
        /// </summary>
        private static IEnumerable<string> InternalFindFiles(string directory, string searchPattern, bool searchSubdirectories)
        {
            if (!Directory.Exists(directory))
                return Enumerable.Empty<string>();

            var patterns = string.IsNullOrWhiteSpace(searchPattern) ? new[] { "*.*" } : searchPattern
                            .Split(Separators.Semicolon, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.StartsWith("*") ? p : $"*{p.Trim()}")
                            .ToArray();
            var option = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return patterns.SelectMany(pat => SafeEnumerateFiles(directory, pat, option)).Distinct(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// .NET Framework 4.x 专用：遇到无权限子目录返回空，不炸。
        /// </summary>
        private static IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern, SearchOption option)
        {
            try
            {
                return Directory.EnumerateFiles(path, searchPattern, option);
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
            catch (DirectoryNotFoundException)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}