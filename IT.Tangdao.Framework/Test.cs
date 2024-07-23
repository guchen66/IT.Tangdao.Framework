using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public class Test
    {
        public static void Usage()
        {
            var container=new TangdaoContainer();
            container.RegisterScoped<ITangdaoClient>();

            var provider=container.Builder();
            var client=provider.Resolve<ITangdaoClient>();

            container.RegisterPlcServer(plc => 
            {
                plc.PlcType= PlcType.Siemens;
                plc.PlcIpAddress = "127.0.0.1";
                plc.Port = "502";

            });
        }
    }
}
