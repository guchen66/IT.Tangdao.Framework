using IT.Tangdao.Framework.DaoDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.IServices
{
    public interface IFileReadService
    {
        Task<TangdaoResponse> ReadLocalFile(string filePath);
    }
}
