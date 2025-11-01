using IT.Tangdao.Framework.Component;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Alarms;
using IT.Tangdao.Framework.Ioc;
using System.Windows;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Configurations;
using System.Windows.Threading;
using IT.Tangdao.Framework.Attributes;
using System.Reflection;
using IT.Tangdao.Framework.Threading;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Bootstrap
{
    internal class ViewModelFirstComponentModule : TangdaoModuleBase
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(ViewModelFirstComponentModule));

        public override void RegisterServices(ITangdaoContainer container)
        {
        }

        public override void OnInitialized(ITangdaoProvider provider)
        {
        }

        public static void Configure()
        {
            var parameter = TangdaoContext.GetTangdaoParameter(nameof(AutoViewAttribute));
            var typeAll = parameter.Get<Type>(nameof(AutoViewAttribute));
            Logger.WriteLocal("对方的ViewModel：" + typeAll.FullName);
            //string assemblyName = "TangdaoDemo";
            //var assembly = Assembly.Load(assemblyName);
            // var s1=typeAll.Assembly.GetTypes();
            var typeDicts = typeAll.Assembly.GetTypes().Where(type => Attribute.IsDefined(type, typeof(AutoViewAttribute)));
            foreach (var type in typeDicts)
            {
                var ViewModel = TangdaoApplication.Provider.GetService(type);
                BuildInstance(ViewModel);
            }
        }

        private static void BuildInstance(object vm)
        {
            var vmType = vm.GetType();

            var viewTypeName = vmType.FullName
                .Replace("ViewModel", "View")
                .Replace("ViewModels", "Views");

            var viewType = vmType.Assembly.GetType(viewTypeName);
            var view = Activator.CreateInstance(viewType);// TangdaoApplication.Provider.GetService(viewType);
            var key = new DataTemplateKey(vm.GetType());
            var factory = new FrameworkElementFactory(view.GetType());

            Application.Current.Resources[key] = new DataTemplate { DataType = vm.GetType(), VisualTree = factory };
        }
    }
}