using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Devices.Plc;

namespace IT.Tangdao.Framework.Devices
{
    public interface IDeviceContainer
    {
        void Register(PlcOption option);
    }
}