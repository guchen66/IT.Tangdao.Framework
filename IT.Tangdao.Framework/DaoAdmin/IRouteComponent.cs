using IT.Tangdao.Framework.DaoAdmin.Navigates;
using IT.Tangdao.Framework.DaoIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public interface IRouteComponent
    {
        ITangdaoPage ResolvePage(string route);
    }
}