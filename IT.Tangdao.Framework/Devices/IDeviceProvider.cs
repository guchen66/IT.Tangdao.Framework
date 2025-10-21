using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Devices
{
    public interface IDeviceProvider
    {
        Temperature DefaultTemp { get; set; }
    }
}