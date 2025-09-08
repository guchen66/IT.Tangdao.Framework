using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows;
using System.Xaml;

namespace IT.Tangdao.Framework.DaoMarkup
{
    public class RelativeToExtension : MarkupExtension
    {
        public string ElementName { get; set; }
        public RelativePosition Position { get; set; }

        public enum RelativePosition
        {
            Left, Right, Top, Bottom, Center
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var rootProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            if (provideValueTarget?.TargetObject is FrameworkElement targetElement &&
                rootProvider?.RootObject is FrameworkElement rootElement)
            {
                // 在加载完成后计算位置
                targetElement.Loaded += (s, e) =>
                {
                    var relativeElement = rootElement.FindName(ElementName) as FrameworkElement;
                    if (relativeElement != null)
                    {
                        CalculatePosition(targetElement, relativeElement);
                    }
                };

                return 0; // 初始值
            }
            return null;
        }

        private void CalculatePosition(FrameworkElement target, FrameworkElement relativeTo)
        {
            var transform = relativeTo.TransformToVisual(Window.GetWindow(target));
            var position = transform.Transform(new Point(0, 0));

            switch (Position)
            {
                case RelativePosition.Right:
                    Canvas.SetLeft(target, position.X + relativeTo.ActualWidth);
                    Canvas.SetTop(target, position.Y);
                    break;
                    // 其他位置计算...
            }
        }
    }
}