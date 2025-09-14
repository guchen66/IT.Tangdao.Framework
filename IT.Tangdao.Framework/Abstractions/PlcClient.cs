using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public class PlcClient : IPlcClient
    {
        public TcpClient Client => GetSingleClient();

        public TcpClient GetSingleClient()
        {
            TcpClient client = new TcpClient();
            return client;
        }
    }
}
