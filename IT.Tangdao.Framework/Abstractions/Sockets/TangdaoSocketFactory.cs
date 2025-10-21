using IT.Tangdao.Framework.Abstractions.Sockets;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.EventArg;
using IT.Tangdao.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.SocketMessages
{
    public class TangdaoSocketFactory : ITangdaoSocketFactory
    {
        public ITangdaoSocket CreateSocket(NetMode mode, ITangdaoUri uri)
        {
            switch (uri.ConnectionType)
            {
                case NetConnectionType.Tcp:
                    return new TcpTangdaoSocket(mode, uri);

                case NetConnectionType.Udp:
                    return new UdpTangdaoSocket(mode, uri); // 需要实现UDP版本

                case NetConnectionType.Serial:
                    return new SerialTangdaoSocket(mode, uri); // 需要实现串口版本

                default:
                    throw new NotSupportedException($"不支持的连接类型: {uri.ConnectionType}");
            }
        }
    }
}