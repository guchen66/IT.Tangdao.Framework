using IT.Tangdao.Framework.Abstractions.Navigates;
using IT.Tangdao.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public interface IRouteComponent
    {
        ITangdaoPage ResolvePage(string route);
    }
}