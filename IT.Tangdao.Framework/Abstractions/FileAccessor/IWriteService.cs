using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    /// <summary>
    /// 写入文件服务
    /// </summary>
    public interface IWriteService
    {
        /// <summary>
        /// 本地写入Txt文本
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="daoFileType"></param>
        void WriteString(string path, string content, DaoFileType daoFileType = DaoFileType.None);

        /// <summary>
        /// 本地异步写入Txt文本
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="daoFileType"></param>
        /// <returns></returns>
        Task<WriteResult> WriteAsync(string path, string content, DaoFileType daoFileType = DaoFileType.None);

        void WriteEntityToXml<TEntity>(TEntity entity, string path) where TEntity : class, new();

        /// <summary>
        /// 批量写入指定文件
        /// </summary>
        /// <returns></returns>
        WriteResult BatchReadFileAsync(string path, DaoFileType daoFileType = DaoFileType.Txt);

        IWrite Current { get; }
    }
}