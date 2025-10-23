using System;
using System.Collections.Generic;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 专责：文件系统级查询，与内容解析无关。
    /// </summary>
    public interface IFileLocator
    {
        /// <summary>
        /// 返回指定目录下匹配后缀的所有文件路径。
        /// </summary>
        IEnumerable<string> FindFiles(string directoryPath, DaoFileType type = DaoFileType.None, bool searchSubdirectories = true);

        /// <summary>
        /// 返回第一个匹配的文件路径；找不到返回 null。
        /// </summary>
        string FindFirst(string directoryPath, DaoFileType type = DaoFileType.None, bool searchSubdirectories = true);
    }
}