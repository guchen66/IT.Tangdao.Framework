using System.Collections.Generic;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Paths;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 链式查询起点。支持自动或显式指定格式后对内容做节点/实体查询。
    /// 所有方法线程安全且可重复调用（同一实例）。
    /// </summary>
    public interface IContentQueryable
    {
        /// <summary>
        /// 读取的内容
        /// </summary>
        string Content { get; }

        /// <summary>
        /// 读取的路径
        /// </summary>
        string ReadPath { get; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        DaoFileType DetectedType { get; }

        /// <summary>
        /// 读取结果。成功时 IsSuccess 为 true；
        /// 失败时保留原始异常，Content 为 null（空文件内容是空字符串，不返回 null）。
        /// </summary>
        ResponseResult Status { get; }
        IXmlQueryable AsXml();

        IJsonQueryable AsJson();

        IConfigQueryable AsConfig();

        IIniQueryable AsIni();

        IContentQueryable Auto(); // 自动探测
    }
}