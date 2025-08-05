using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows;

namespace IT.Tangdao.Framework.DaoConverters
{
    public class GetWindowExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service?.TargetObject is DependencyObject obj)
                return Window.GetWindow(obj);
            return null;
        }
    }
}