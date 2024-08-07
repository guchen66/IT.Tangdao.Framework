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
        /// <summary>
        /// 读取本地文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task<TangdaoResponse> ReadLocalFile(string filePath);

        /// <summary>
        /// 读取网络文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task<TangdaoResponse> ReadNetFile(string filePath);
    }
}
