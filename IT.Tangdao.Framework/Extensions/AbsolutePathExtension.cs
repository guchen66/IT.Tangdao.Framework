using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Paths;

namespace IT.Tangdao.Framework.Extensions
{
    public static class AbsolutePathExtension
    {
        /// <summary>
        /// 把当前文件复制到同目录、同主文件名、指定扩展名的新文件
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="newExtension">新扩展名（必须含点，例如 ".bak"）</param>
        /// <param name="overwrite">是否覆盖已存在文件</param>
        /// <returns>备份后的 AbsolutePath</returns>
        public static AbsolutePath Backup(this AbsolutePath source, string newExtension, bool overwrite = true)
        {
            if (!source.FileExists)
                throw new FileNotFoundException("源文件不存在", source.Value);

            var backup = source.WithExtension(newExtension);
            File.Copy(source.Value, backup.Value, overwrite);
            return backup;   // 继续链式
        }

        /// <summary>
        /// 直接复制到任意目标路径
        /// </summary>
        public static AbsolutePath CopyTo(this AbsolutePath source, AbsolutePath destination, bool overwrite = true)
        {
            if (!source.FileExists)
                throw new FileNotFoundException("源文件不存在", source.Value);

            Directory.CreateDirectory(destination.Parent().Value); // 确保目录存在
            File.Copy(source.Value, destination.Value, overwrite);
            return destination;
        }

        /// <summary>
        /// 获取路径上指定后缀文件
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IReadOnlyList<AbsolutePath> EnumerateFiles(this AbsolutePath root, string pattern)
        {
            return Directory.EnumerateFiles(root.Value, pattern)
                .Select(p => new AbsolutePath(p))
                .ToArray();
        }
    }
}