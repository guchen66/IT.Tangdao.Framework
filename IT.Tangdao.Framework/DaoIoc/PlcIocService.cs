using IT.Tangdao.Framework.DaoDtos.Options;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoIoc
{
    public class PlcIocService
    {
        public static void RegisterPlcServer(PlcOption option)
        {
            PlcServerCollectionExtension.PlcOptions.Add(option);
        }

       /* public static void RegisterPlcServer(List<IocConfig> iocList)
        {
            SugarServiceCollectionExtensions.configs.AddRange(iocList);
        }

        public static void ConfigurationPlc(Action<SqlSugarClient> action)
        {
            SugarServiceCollectionExtensions.Configuration = action;
        }*/
    }
}
