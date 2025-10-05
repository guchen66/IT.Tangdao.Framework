using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows;

namespace IT.Tangdao.Framework.Markup
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
                    // 或者使用依赖注入容器
                    return TangdaoApplication.Provider.GetService(ServiceType);
                }
            }
            return null;
        }
    }
}