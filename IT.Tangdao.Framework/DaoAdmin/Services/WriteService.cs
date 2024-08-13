using IT.Tangdao.Framework.DaoAdmin.IServices;
using IT.Tangdao.Framework.DaoDtos;
using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.Extensions;
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
        public void WriteString(string path,string content, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            path.UseFileWriteToTxt(content);
        }

        public async Task<ReadOrWriteResult> WriteAsync(string path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            await new TimeSpan(1000);
            path.UseFileWriteToTxt(content);
            return new ReadOrWriteResult(true,content);
        }

        public Task<string> WriteFilterAsync(string path, Expression<Func<string, bool>> func)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> WriteXmlToEntityAsync<TEntity>(string path, DaoFileType daoFileType) where TEntity : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
