using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.DaoDevices.Plc;
using IT.Tangdao.Framework.DaoCommon;

namespace IT.Tangdao.Framework.DaoDevices
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