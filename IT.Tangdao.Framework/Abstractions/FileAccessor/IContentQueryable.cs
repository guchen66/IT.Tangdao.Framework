using System.Collections.Generic;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 链式查询起点。支持自动或显式指定格式后对内容做节点/实体查询。
    /// 所有方法线程安全且可重复调用（同一实例）。
    /// </summary>
    public interface IContentQueryable
    {
        string Content { get; }

        IContentXmlQueryable AsXml();

        IContentJsonQueryable AsJson();

        IContentConfigQueryable AsConfig();

        IContentQueryable Read(string path, DaoFileType t = DaoFileType.None);

        //Task<IContentQueryable> ReadAsync(string path, DaoFileType daoFileType = DaoFileType.None);

        IContentQueryable Auto();          // 自动探测

        IContentQueryable this[int index] { get; }

        IContentQueryable this[string readObject] { get; }
    }
}