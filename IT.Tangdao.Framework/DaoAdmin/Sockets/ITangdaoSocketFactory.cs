using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.DaoParameters.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.SocketMessages
{
    public interface ITangdaoSocketFactory
    {
        ITangdaoSocket CreateSocket(NetMode mode, ITangdaoUri uri);
    }
}