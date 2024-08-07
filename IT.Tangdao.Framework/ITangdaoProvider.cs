using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public interface ITangdaoProvider: ITangdaoProviderBuilder
    {
        object Resolve(Type type);
        object Resolve(Type type,bool useFactoryMethods);
       // object Resolve(Type type, params (Type Type, object Instance)[] parameters);
    }
}
