using IT.Tangdao.Framework.DaoComponents;
using IT.Tangdao.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework;
namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoContainerExtension
    {
        public static ITangdaoContainer RegisterType<TService, TImplementation>(this ITangdaoContainer container) where TImplementation : TService
        {
            return container.Register(typeof(TService), typeof(TImplementation));
        }

        public static ITangdaoContainer RegisterType<TService>(this ITangdaoContainer container, Func<object> creator)
        {
            return container.Register(typeof(TService), creator);
        }

        public static ITangdaoContainer RegisterType<TService>(this ITangdaoContainer container, Func<ITangdaoProvider, object> factoryMethod)
        {
            return container.Register(typeof(TService), factoryMethod);
        }
    }
}
