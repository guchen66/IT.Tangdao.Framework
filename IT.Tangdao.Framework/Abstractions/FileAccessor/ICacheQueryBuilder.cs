using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 缓存查询构建器，用于带缓存的内容读取
    /// </summary>
    public interface ICacheQueryBuilder : IContentBuilder
    {
        /// <summary>
        /// 清除指定路径的缓存
        /// </summary>
        void Clear(string path);

        /// <summary>
        /// 清除整个区域的缓存
        /// </summary>
        void ClearRegion(string region);
    }
}