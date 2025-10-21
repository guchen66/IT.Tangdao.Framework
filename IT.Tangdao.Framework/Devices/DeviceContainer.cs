using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Devices.Plc;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Devices
{
    public class DeviceContainer
    {
        public static DeviceContainer Default { get; } = new DeviceContainer();

        public void Register(Func<PlcOption> action)
        {
            var option = action.Invoke();
            RegisterContext registerContext = new RegisterContext();
            registerContext.Option = option;
            TangdaoContext.SetContext<PlcOption>(registerContext);
        }
    }
}