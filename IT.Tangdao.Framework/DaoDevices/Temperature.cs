using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDevices
{
    /// <summary>
    /// 温度计
    /// </summary>
    public class Temperature
    {
        public int Id { get; set; }

        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public Parity Parity { get; set; }

        public int DataBits { get; set; }

        public StopBits StopBits { get; set; }
    }
}