using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using IT.Tangdao.Framework.DaoConverters;
using IT.Tangdao.Framework.DaoConverters.Wpf;

namespace IT.Tangdao.Framework.DaoMarkup
{
    public class IndexWithMultiBindingExtension : MarkupExtension
    {
        public object AdditionalBinding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var ppt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var target = ppt.TargetObject as FrameworkElement;
            if (target == null) return null;

            string idx = (target.Parent is Panel p) ? p.Children.IndexOf(target).ToString() : "-1";

            var mb = new MultiBinding
            {
                Converter = new IndexAndDataConverter()
            };
            mb.Bindings.Add(new Binding { Source = idx, Mode = BindingMode.OneTime });
            mb.Bindings.Add(new Binding());                          // DataContext
            if (AdditionalBinding is BindingBase ab)
                mb.Bindings.Add(ab);

            // 不直接 return mb，而是**通过附加属性挂出去**，避免塞给 string
            BindingOperations.SetBinding(target, TagProperty, mb);   // 先写到 Tag
            return DependencyProperty.UnsetValue;                    // 不污染原属性
        }

        private static readonly DependencyProperty TagProperty =
            DependencyProperty.RegisterAttached("IndexWithMultiBinding", typeof(object), typeof(IndexWithMultiBindingExtension));
    }
}