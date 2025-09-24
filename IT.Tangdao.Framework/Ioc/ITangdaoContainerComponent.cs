using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Component;

namespace IT.Tangdao.Framework.Ioc
{
    internal interface ITangdaoContainerComponent : ITangdaoComponent
    {
        void Load(ITangdaoContainer container, DaoComponentContext context);
    }
}