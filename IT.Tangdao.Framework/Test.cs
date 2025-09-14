using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.IServices;
using IT.Tangdao.Framework.Abstractions.Services;
using IT.Tangdao.Framework.Enums;
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
            container.RegisterType<ITangdaoClient, TangdaoClient>();
            container.RegisterViewToViewModel("win");
          /*  var provider=container.Builder();
            var client=provider.Resolve<ITangdaoClient>();
*/
            container.RegisterPlcServer(plc => 
            {
                plc.PlcType= PlcType.Siemens;
                plc.PlcIpAddress = "127.0.0.1";
                plc.Port = "502";

            });

          /*  container.RegisterType<IPlcReadService,PlcReadService>();
            var plcservice=provider.Resolve<IPlcReadService>();
            plcservice.ReadAsync("地址");*/
            
        }
    }
}
