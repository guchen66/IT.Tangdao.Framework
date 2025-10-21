using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml;

namespace IT.Tangdao.Framework.Markup
{
    /// <summary>
    /// 针对复杂的绑定，直接返回VM
    /// </summary>
    public class AncestorBindingExtension : MarkupExtension
    {
        /// <summary>
        /// 最终要绑定的路径，默认拿 VM 本身
        /// </summary>
        public PropertyPath Path { get; set; }

        /// <summary>
        /// 是否优先查找特定类型的父级
        /// </summary>
        public Type AncestorType { get; set; } = typeof(UserControl);

        public override object ProvideValue(IServiceProvider sp)
        {
            var rootProvider = sp.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            if (rootProvider != null)
            {
                var rootObject = rootProvider.RootObject;
                AncestorType = rootObject.GetType();
            }
            return new Binding
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = AncestorType,
                    AncestorLevel = 1
                },
                Path = Path
            }.ProvideValue(sp);
        }
    }
}