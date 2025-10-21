using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Devices.Plc
{
    public class PlcOption
    {
        public PlcType PlcType { get; set; }

        public string PlcIpAddress { get; set; }

        public string Port { get; set; }

        public bool IsAutoConnection { get; set; }

        public List<PlcOption> SlaveConnectionOptions { get; set; }
    }
}