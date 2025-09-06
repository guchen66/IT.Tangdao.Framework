using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Sockets
{
    public interface ITangdaoResponse
    {
        event EventHandler<string> Received;

        event EventHandler<Exception> Error;
    }
}