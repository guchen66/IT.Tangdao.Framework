using IT.Tangdao.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public class TangdaoScope : ITangdaoScope
    {
        internal TangdaoProvider _tangdaoProvider;

        public static object FromContainerType(Type type)
        {
            return ManualDependProvider.ResolveDependLinkList(type);
        }

        public object Resolve(Type type)
        {
            return _tangdaoProvider.Resolve(type);
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
