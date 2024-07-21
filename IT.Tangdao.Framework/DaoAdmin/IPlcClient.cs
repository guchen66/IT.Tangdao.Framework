using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public interface IPlcClient
    {
        TcpClient Client { get; }
    }
}
