using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Abstractions.Services;
using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.IServices
{
    /// <summary>
    /// 定义读取文本的服务
    /// </summary>
    public interface IReadService
    {
        /// <summary>
        /// 读取指定地址
        /// </summary>
        /// <param name="path"></param>
        /// <param name="daoFileType"></param>
        /// <returns></returns>
        string Read(string path, DaoFileType daoFileType = DaoFileType.None);

        /// <summary>
        /// 异步读取指定地址
        /// </summary>
        /// <param name="path"></param>
        /// <param name="daoFileType"></param>
        /// <returns></returns>
        Task<string> ReadAsync(string path, DaoFileType daoFileType = DaoFileType.None);

        /// <summary>
        /// 读取XML文件并转成对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="path"></param>
        /// <param name="daoFileType"></param>
        /// <returns></returns>
        Task<TEntity> ReadXmlToEntityAsync<TEntity>(string path, DaoFileType daoFileType) where TEntity : class, new();

        /// <summary>
        /// 批量读取指定文件
        /// </summary>
        /// <returns></returns>
        ReadResult<string> BatchReadFileAsync(string path, DaoFileType daoFileType = DaoFileType.Txt);

        /// <summary>
        /// 读取文本的内置接口
        /// </summary>
        IRead Current { get; }
    }
}