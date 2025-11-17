using System;
using System.Collections.Generic;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Paths;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 专责：文件系统级查询，与内容解析无关。
    /// </summary>
    public interface IFileLocator
    {
        /// <summary>
        /// 返回指定目录下匹配后缀的所有文件路径。
        /// 默认不搜索子目录
        /// </summary>
        IEnumerable<string> FindFiles(string directoryPath, DaoFileType type = DaoFileType.None, bool searchSubdirectories = false);

        /// <summary>
        /// 返回指定目录下匹配后缀的所有文件路径。
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="type"></param>
        /// <param name="searchSubdirectories"></param>
        /// <returns></returns>
        IEnumerable<string> FindFiles(AbsolutePath absolutePath, DaoFileType type = DaoFileType.None, bool searchSubdirectories = false);

        /// <summary>
        /// 返回第一个匹配的文件路径；找不到返回 null。
        /// 默认不搜索子目录
        /// </summary>
        string FindFirst(string directoryPath, DaoFileType type = DaoFileType.None, bool searchSubdirectories = false);
    }
}