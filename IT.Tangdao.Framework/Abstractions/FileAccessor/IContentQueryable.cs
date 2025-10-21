using System.Collections.Generic;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Selectors;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Abstractions
{
    /// <summary>
    /// 链式查询起点。支持自动或显式指定格式后对内容做节点/实体查询。
    /// 所有方法线程安全且可重复调用（同一实例）。
    /// </summary>
    public interface IContentQueryable
    {
        /// <summary>自动探测格式并返回自身。</summary>
        IContentQueryable Auto();

        /// <summary>强制按 Xml 解析后续操作。</summary>
        IContentQueryable AsXml();

        /// <summary>强制按 Json 解析后续操作。</summary>
        IContentQueryable AsJson();

        /// <summary>强制按 Config 解析后续操作。</summary>
        IContentQueryable AsConfig();

        IContentQueryable this[int readIndex] { get; }

        IContentQueryable Read(string path, DaoFileType daoFileType = DaoFileType.None);

        // --------- 业务查询 ----------
        ReadResult SelectNode(string node);

        ReadResult<T> Select<T>(string key = null);

        ReadResult<List<T>> SelectNodes<T>() where T : new();
    }
}