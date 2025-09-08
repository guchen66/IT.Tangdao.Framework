using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;

namespace IT.Tangdao.Framework.DaoMarkup
{
    public class GetParentChainExtension : MarkupExtension
    {
        public Type ParentType { get; set; }
        public int MaxDepth { get; set; } = 10;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (provideValueTarget?.TargetObject is DependencyObject current)
            {
                var parents = new List<DependencyObject>();
                var depth = 0;

                while (current != null && depth < MaxDepth)
                {
                    current = VisualTreeHelper.GetParent(current);
                    if (current != null)
                    {
                        if (ParentType == null || current.GetType() == ParentType)
                        {
                            parents.Add(current);
                        }
                    }
                    depth++;
                }
                return parents;
            }
            return null;
        }
    }
}