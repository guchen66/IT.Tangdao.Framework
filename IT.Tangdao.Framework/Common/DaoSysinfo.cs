using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoCommon
{
    public static class DaoSysinfo
    {
        public static string CurrentAppName => QueryableAppName();

        public static string QueryableAppName()
        {
            return System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().GetName().Name); 
        }
    }
}
