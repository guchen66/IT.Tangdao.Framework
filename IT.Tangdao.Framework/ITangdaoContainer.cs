using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public interface ITangdaoContainer : ITangdaoContainerBuilder
    {
        ITangdaoProvider Builder();

        ITangdaoContainer Register(Type serviceType, Type implementationType);

        ITangdaoContainer Register(Type implementationType);

        ITangdaoContainer Register(Type serviceType, Func<object> creator);

        ITangdaoContainer Register(Type type, Func<ITangdaoProvider, object> factoryMethod);

        ITangdaoContainer Register(string name);

        ITangdaoContainerBuilder Register<TService, TImplementation>() where TImplementation : TService;

        ITangdaoContainerBuilder Register<TImplementation>();
    }
}