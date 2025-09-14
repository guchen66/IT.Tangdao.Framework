using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace IT.Tangdao.Framework.Markup
{
    public class DynamicResourceExExtension : MarkupExtension
    {
        public string ResourceKey { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            //if (provideValueTarget?.TargetObject is DependencyObject targetObject)
            //{
            //    var binding = new Binding()
            //    {
            //        Source = targetObject,
            //        Path = new PropertyPath("(0)", DynamicResourceExtension.ResourceKeyProperty),
            //        Converter = Converter,
            //        ConverterParameter = ConverterParameter,
            //        Mode = BindingMode.OneWay
            //    };

            //    // 设置绑定表达式来动态获取资源
            //    BindingOperations.SetBinding(targetObject,
            //        DynamicResourceExtension.ResourceKeyProperty,
            //        new Binding() { Source = ResourceKey });

            //    return binding.ProvideValue(serviceProvider);
            //}
            return null;
        }
    }
}