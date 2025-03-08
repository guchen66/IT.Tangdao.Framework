using IT.Tangdao.Framework.DaoComponents;
using IT.Tangdao.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework;
using System.Windows;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoContainerExtension
    {
        public static ITangdaoContainer RegisterType<TService, TImplementation>(this ITangdaoContainer container) where TImplementation : TService
        {
            return container.Register(typeof(TService), typeof(TImplementation));
        }

        public static ITangdaoContainer RegisterType<TImplementation>(this ITangdaoContainer container)
        {
            return container.Register(typeof(TImplementation));
        }

        public static ITangdaoContainer RegisterType<TService>(this ITangdaoContainer container, Func<object> creator)
        {
            return container.Register(typeof(TService), creator);
        }

        public static ITangdaoContainer RegisterType<TService>(this ITangdaoContainer container, Func<ITangdaoProvider, object> factoryMethod)
        {
            return container.Register(typeof(TService), factoryMethod);
        }

        public static ITangdaoContainer RegisterViewToViewModel(this ITangdaoContainer container, string name = null)
        {
            return container.Register(name);
        }

        /// <summary>
        /// Window未打开之前，不能给它设置子窗体
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <typeparam name="TChildWindow"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
       /* public static Window RegisterOwner<TWindow,TChildWindow>(this ITangdaoContainer container) where TWindow:Window, new() where TChildWindow : Window, new()
        {
            Window window = new TChildWindow();
            window.Owner=new TWindow();
            return window;
        }*/
    }
}