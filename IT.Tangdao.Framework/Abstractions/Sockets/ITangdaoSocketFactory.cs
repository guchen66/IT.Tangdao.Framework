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
    public interface ITangdaoSocketFactory
    {
        ITangdaoSocket CreateSocket(NetMode mode, ITangdaoUri uri);
    }
}