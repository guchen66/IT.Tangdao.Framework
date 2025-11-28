using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System.Xaml;
using System.Windows.Controls;

namespace IT.Tangdao.Framework.Markup
{
    public class RelativeBindingExtension : MarkupExtension
    {
        /// <summary>
        /// 相对路径网上查找绑定的路径，默认拿 VM 本身
        /// </summary>
        public PropertyPath Path { get; set; }

        public int AncestorLevel { get; set; } = 1;

        /// <summary>
        /// 是否优先查找特定类型的父级
        /// </summary>
        public string AncestorTypeName { get; set; } = "UserControl";

        public override object ProvideValue(IServiceProvider sp)
        {
            var typeResolver = sp.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
            var ancestorType = typeResolver?.Resolve(AncestorTypeName) ?? typeof(UserControl);

            return new Binding
            {
                Path = Path,
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = ancestorType,
                    AncestorLevel = AncestorLevel
                }
            }.ProvideValue(sp);
        }
    }
}