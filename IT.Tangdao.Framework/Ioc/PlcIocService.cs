using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Extensions;

namespace IT.Tangdao.Framework.Ioc
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