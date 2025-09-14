using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Parameters.EventArg;
using IT.Tangdao.Framework.Parameters.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.SocketMessages
{
    public interface ITangdaoSocketFactory
    {
        ITangdaoSocket CreateSocket(NetMode mode, ITangdaoUri uri);
    }
}