using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDevices
{
    public class DeviceProvider : DeviceBase, IDeviceProvider
    {
        public Temperature DefaultTemp { get; set; }

        public DeviceProvider()
        {
        }
    }
}