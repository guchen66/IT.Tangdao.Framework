using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDevices
{
    public class DeviceBase
    {
        public Temperature GetTemperature(IDeviceProvider device)
        {
            return device.DefaultTemp;
        }
    }
}