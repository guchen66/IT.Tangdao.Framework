using IT.Tangdao.Framework.DaoAdmin.IServices;
using IT.Tangdao.Framework.DaoDtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Services
{
    public class FileReadService : IFileReadService
    {
        /// <summary>
        /// 读取本地的TXT文件，以后在扩展
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<TangdaoResponse> ReadLocalFile(string filePath)
        {
            if (filePath !=null)
            {
                using (var sr = new StreamReader(filePath))
                {
                    var result=await sr.ReadToEndAsync();
                    return new TangdaoResponse(result, true);
                }

            }
            return new TangdaoResponse(false, AlarmBackResult.Failt);
        }

        public Task<TangdaoResponse> ReadNetFile(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
