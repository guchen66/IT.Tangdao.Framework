using IT.Tangdao.Framework.DaoCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public abstract class TangdaoAdapter
    {
        public readonly List<CommonContext> CurrentContext = new List<CommonContext>();

        // public abstract ITangdaoContainer Register(Type serviceType, Type implementationType, List<CommonContext> commonContexts);
    }
}
