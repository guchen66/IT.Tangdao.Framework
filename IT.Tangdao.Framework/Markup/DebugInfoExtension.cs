using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows;
using System.Xaml;

namespace IT.Tangdao.Framework.Markup
{
    public class DebugInfoExtension : MarkupExtension
    {
        public string Tag { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var rootProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            if (provideValueTarget?.TargetObject is DependencyObject targetObject)
            {
#if DEBUG
            // 添加调试信息
            Debug.WriteLine($"DebugInfo - Tag: {Tag}, Target: {targetObject.GetType().Name}");

            // 可以在设计时显示额外信息
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(targetObject))
            {
                return $"[DEBUG: {Tag}]";
            }
#endif
                return "Production Value";
            }
            return null;
        }
    }
}