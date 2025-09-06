using IT.Tangdao.Framework.DaoAdmin.Results;
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
        Task<ReadResult> ReadLocalFile(string filePath);

        /// <summary>
        /// 读取网络文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task<ReadResult> ReadNetFile(string filePath);
    }
}