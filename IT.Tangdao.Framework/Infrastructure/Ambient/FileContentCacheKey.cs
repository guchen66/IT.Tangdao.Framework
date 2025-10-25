using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Infrastructure.Ambient
{
    public class FileContentCacheKey
    {
        public static string Create(string path, DaoFileType type)
        {
            // 与 ContentQueryable 同一套根 key
            return string.Format("Content:{0}:{1}", path, type);
        }
    }
}