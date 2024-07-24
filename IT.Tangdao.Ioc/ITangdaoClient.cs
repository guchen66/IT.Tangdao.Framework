using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Ioc
{
    public interface ITangdaoClient:IDisposable
    {
        TcpClient Client { get; }
    }
}
