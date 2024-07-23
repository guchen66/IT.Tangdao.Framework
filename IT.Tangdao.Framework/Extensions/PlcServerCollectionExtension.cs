using IT.Tangdao.Framework.DaoAdmin;
using IT.Tangdao.Framework.DaoDtos.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class PlcServerCollectionExtension
    {
        internal static List<PlcOption> PlcOptions=new List<PlcOption>();

        public static IPlcBuilder RegisterPlcServer(this ITangdaoContainer container)
        {
            return container.RegisterPlcIsOnLine();        //首先我注册PLC服务，第一步查看PLC是否在线
        }     

        public static IPlcBuilder RegisterPlcIsOnLine(this ITangdaoContainer container)
        {
            var builder = RegisterPlcServerCore(container);
            return new PlcBuilder(builder.Container);
        }

        public static IPlcBuilder RegisterPlcServerCore(this ITangdaoContainer container)
        {
            var builder =container.RegisterPlcMode();
            return builder;
        }
        public static IPlcBuilder RegisterPlcMode(this ITangdaoContainer container)
        {
            var builder = container.RegisterPlcMode();
            return builder;
        }


        public static IPlcBuilder RegisterPlcServer(this ITangdaoContainer container, Action<PlcOption> action = null)
        {
            var builder=container.RegisterPlcServerCore();
            if (action!=null)
            {
              //  builder.RegisterOption(action);
            }
            return builder;
        }
    }
}
