using IT.Tangdao.Framework.DaoAdmin.Services;
using IT.Tangdao.Framework.DaoEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.IServices
{
    /// <summary>
    /// 定义读取文本的服务
    /// </summary>
    public interface IReadService
    {
        string Read(string path, DaoFileType daoFileType = DaoFileType.None);

        Task<string> ReadAsync(string path, DaoFileType daoFileType = DaoFileType.None);

        Task<TEntity> ReadXmlToEntityAsync<TEntity>(string path, DaoFileType daoFileType) where TEntity : class, new();

        Task<string> QueryFilterAsync(string path, Expression<Func<string, bool>> func);

        IRead Current { get; }

        IHardwaredevice Device { get; }
    }
}