using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows;

namespace IT.Tangdao.Framework.DaoMarkup
{
    public class ServiceLocatorExtension : MarkupExtension
    {
        public Type ServiceType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (provideValueTarget?.TargetObject is DependencyObject targetObject)
            {
                var window = Window.GetWindow(targetObject);
                if (window != null)
                {
                    // 假设你的 Window 实现了 IServiceProvider 或有服务容器
                    if (window is IServiceProvider serviceProviderWindow)
                    {
                        return serviceProviderWindow.GetService(ServiceType);
                    }

                    // 或者使用依赖注入容器
                    // return YourDIContainer.GetService(ServiceType);
                }
            }
            return null;
        }
    }
}