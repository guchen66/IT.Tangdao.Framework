using IT.Tangdao.Framework.DaoEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDtos.Options
{
    public class PlcOption
    {
        public PlcType PlcType { get;  set; }

        public string PlcIpAddress { get;  set; }

        public string Port { get;  set; }

        public bool IsAutoConnection { get; set; }

        public List<PlcOption> SlaveConnectionOptions { get; set; }
    }
}
