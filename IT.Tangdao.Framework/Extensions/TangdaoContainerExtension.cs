using IT.Tangdao.Framework.DaoComponents;
using IT.Tangdao.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoContainerExtension
    {
        public static ITangdaoContainer RegisterScoped(this ITangdaoContainer container, Type type)
        {
            var componentContextLinkList = ManualDependProvider.CreateDependLinkList(type);

            foreach (var daoComponentContext in componentContextLinkList)
            {
                var item = Activator.CreateInstance(daoComponentContext.ComponentType);
            }

            return container;
        }

        public static ITangdaoContainer RegisterScoped(this ITangdaoContainer container, Type type, object imple)
        {
            var componentContextLinkList = ManualDependProvider.CreateDependLinkList(type, imple);

            foreach (var daoComponentContext in componentContextLinkList)
            {
                var item = Activator.CreateInstance(daoComponentContext.ComponentType);
            }

            return container;
        }

        //注册接口的实现类
        public static ITangdaoContainer RegisterSingleton<TImple>(this ITangdaoContainer container, TImple imple) where TImple : class 
        {
            return container.RegisterSingleton(typeof(TImple), imple);
        }

        public static ITangdaoContainer RegisterSingleton(this ITangdaoContainer container, Type type,object imple) 
        {
            var componentContextLinkList=ManualDependProvider.CreateDependLinkList(type,imple);

            foreach (var daoComponentContext in componentContextLinkList)
            {
                var item=Activator.CreateInstance(daoComponentContext.ComponentType);
            }

            return container;
        }
    }
}
