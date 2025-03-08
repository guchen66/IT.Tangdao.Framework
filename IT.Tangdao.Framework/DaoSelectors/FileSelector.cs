using IT.Tangdao.Framework.DaoAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoSelectors
{
    public class FileSelector
    {
        /// <summary>
        /// 文件查询
        /// </summary>
        /// <returns></returns>
        public static IRead Queryable()
        {
            return new Read();
        }

        /// <summary>
        /// 文件导入
        /// </summary>
        public static void Import()
        {
        }

        /// <summary>
        /// 文件导出
        /// </summary>
        public static void Export()
        {
        }
    }
}