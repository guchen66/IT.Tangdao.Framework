using IT.Tangdao.Framework.DaoDtos;
using IT.Tangdao.Framework.DaoEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.IServices
{
    public interface IWriteService
    {
        void WriteString(string path,string content, DaoFileType daoFileType = DaoFileType.None);

        Task<ReadOrWriteResult> WriteAsync(string path,string content, DaoFileType daoFileType = DaoFileType.None);

        Task<TEntity> WriteXmlToEntityAsync<TEntity>(string path, DaoFileType daoFileType) where TEntity : class, new();

        Task<string> WriteFilterAsync(string path, Expression<Func<string, bool>> func);
    }
}
