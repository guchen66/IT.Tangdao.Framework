using IT.Tangdao.Framework.DaoAdmin.IServices;
using IT.Tangdao.Framework.DaoAdmin.Results;
using IT.Tangdao.Framework.DaoDtos;
using IT.Tangdao.Framework.DaoDtos.Globals;
using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Services
{
    public class WriteService : IWriteService
    {
        public void WriteString(string path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            path.UseFileWriteToTxt(content);
        }

        public async Task<WriteResult> WriteAsync(string path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            await new TimeSpan(1000);
            path.UseFileWriteToTxt(content);
            return WriteResult.Success(content);
        }

        public void WriteFilter(string path, Expression<Func<string, bool>> func)
        {
            throw new NotImplementedException();
        }

        public void WriteEntityToXml<TEntity>(TEntity entity, string path) where TEntity : class, new()
        {
            if (path != null)
            {
                var info = XmlFolderHelper.SerializeXML<TEntity>(entity);
                WriteString(path, info);
            }
            else
            {
                TangdaoGuards.ThrowIfNull(entity);
            }
        }
    }
}