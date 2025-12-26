using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Paths;
using IT.Tangdao.Framework.Configurations;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IContentBuilder
    {
        IContentQueryable Read(string path, DaoFileType t = DaoFileType.None);

        IContentQueryable Read(AbsolutePath path, DaoFileType t = DaoFileType.None);

        IContentWritable Write(string path, string content, DaoFileType daoFileType = DaoFileType.None);

        IContentWritable Write(AbsolutePath path, string content, DaoFileType daoFileType = DaoFileType.None);

        /// <summary>
        /// 获取空的内容查询器，用于不需要读取文件的情况（如配置文件读取）
        /// </summary>
        IContentQueryable Empty();
    }
}