using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using IT.Tangdao.Framework.Abstractions.Contracts;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ViewModelTemplateExtension
    {
        /// <summary>
        /// 自动创建 DataTemplate 并注入全局资源。
        /// </summary>
        public static FrameworkElement AddTemplate<TVM, TView>(this FrameworkElement vm) where TVM : FrameworkElement where TView : FrameworkElement, new()
        {
            // ① 创建代码级 DataTemplate
            var template = new DataTemplate
            {
                DataType = typeof(TVM),
                VisualTree = new FrameworkElementFactory(typeof(TView))
            };
            DataTemplateKey dataTemplateKey = new DataTemplateKey();
            // ② 丢进全局资源字典（没有 Key，就是隐式模板）
            Application.Current.Resources.Add(template.DataType, template);

            return vm;
        }
    }
}