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
        ///默认不搜索子目录
        /// </summary>
        /// <param name="directoryPath">搜索的地址</param>
        /// <param name="searchPattern">搜索的后缀</param>
        /// <param name="searchSubdirectories">是否搜索子目录</param>
        /// <returns></returns>
        IEnumerable<string> FindFiles(string directoryPath, string searchPattern, bool searchSubdirectories = false);

        /// <summary>
        /// 返回指定目录下匹配后缀的所有文件路径。
        /// </summary>
        /// <param name="absolutePath">搜索的地址</param>
        /// <param name="searchPattern">搜索的后缀</param>
        /// <param name="searchSubdirectories">是否搜索子目录</param>
        /// <returns></returns>
        IEnumerable<string> FindFiles(AbsolutePath absolutePath, string searchPattern, bool searchSubdirectories = false);

        /// <summary>
        /// 返回第一个匹配的文件路径；找不到返回 null。
        /// </summary>
        /// <param name="directoryPath">搜索的地址</param>
        /// <param name="searchPattern">搜索的后缀</param>
        /// <param name="searchSubdirectories">是否搜索子目录</param>
        /// <returns></returns>
        string FindFirst(string directoryPath, string searchPattern, bool searchSubdirectories = false);

        /// <summary>
        /// 返回第一个匹配的文件路径；找不到返回 null。
        /// </summary>
        /// <param name="absolutePath">搜索的地址</param>
        /// <param name="searchPattern">搜索的后缀</param>
        /// <param name="searchSubdirectories">是否搜索子目录</param>
        /// <returns></returns>
        string FindFirst(AbsolutePath absolutePath, string searchPattern, bool searchSubdirectories);
    }
}