using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface ICacheContentQueryable : IContentQueryable
    {
        /// 唯一一个原子方法：拿带缓存的内容
        new IContentQueryable Read(string path, DaoFileType type = DaoFileType.None);

        /// 清除一条缓存
        void Clear(string path);

        /// 清除整个区域（可选）
        void ClearRegion(string region);
    }
}