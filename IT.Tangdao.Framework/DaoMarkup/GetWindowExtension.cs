using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows;

namespace IT.Tangdao.Framework.DaoMarkup
{
    /// <summary>
    /// 获取当前控件所在的Window实例
    /// </summary>
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