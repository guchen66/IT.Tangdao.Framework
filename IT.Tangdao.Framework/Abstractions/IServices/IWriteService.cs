using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.DaoDtos;
using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.IServices
{
    public interface IWriteService
    {
        void WriteString(string path, string content, DaoFileType daoFileType = DaoFileType.None);

        Task<WriteResult> WriteAsync(string path, string content, DaoFileType daoFileType = DaoFileType.None);

        void WriteEntityToXml<TEntity>(TEntity entity, string path) where TEntity : class, new();

        void WriteFilter(string path, Expression<Func<string, bool>> func);
    }
}