using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xaml;

namespace IT.Tangdao.Framework.Markup
{
    /// <summary>
    /// 获取当前控件所在的Window实例
    /// 失败时返回null
    /// </summary>
    public class GetWindowExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // IProvideValueTarget：WPF服务接口，用于获取XAML中正在被赋值的"目标对象"和"目标属性"
            var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            //获取目标的顶级窗体类型
            var rootProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            var rootObject = rootProvider?.RootObject;

            // 从目标对象向上遍历可视化树，返回所属的Window
            if (service?.TargetObject is FrameworkElement element)
            {
                // 情况1：根对象是Window，说明控件直接在Window中，可以直接获取
                if (rootObject is Window)
                {
                    var window = Window.GetWindow(element);
                    return window;
                }
                // 返回延迟加载代理，解决XAML解析时视觉树未完全建立的时序问题
                return new WindowProviderImpl(element);
            }

            // 若获取失败（目标对象为null/不在可视化树/Window未构建），返回null
            return null;
        }

        /// <summary>
        /// Window延迟加载代理类
        /// 通过隐式转换实现延迟获取，避免MarkupExtension解析时序问题
        /// </summary>
        private class WindowProviderImpl
        {
            private readonly FrameworkElement _element;
            private Window _cachedWindow;
            private bool _isResolved;

            public WindowProviderImpl(FrameworkElement element)
            {
                _element = element;
            }

            /// <summary>
            /// 获取Window实例
            /// </summary>
            private Window GetWindow()
            {
                // 已缓存则直接返回
                if (_isResolved)
                    return _cachedWindow;

                // 尝试直接获取
                var window = Window.GetWindow(_element);

                if (window != null)
                {
                    _cachedWindow = window;
                    _isResolved = true;
                    return window;
                }

                // 直接获取失败，使用DispatcherFrame延迟等待视觉树建立
                var frame = new DispatcherFrame();
                Window result = null;

                RoutedEventHandler loadedHandler = null;
                loadedHandler = (s, e) =>
                {
                    result = Window.GetWindow(_element);
                    _element.Loaded -= loadedHandler;
                    frame.Continue = false;
                };

                _element.Loaded += loadedHandler;

                if (_element.IsLoaded)
                {
                    result = Window.GetWindow(_element);
                    _element.Loaded -= loadedHandler;
                }
                else
                {
                    Dispatcher.PushFrame(frame);
                }

                _cachedWindow = result;
                _isResolved = true;
                return result;
            }

            /// <summary>
            /// 隐式转换为Window
            /// 当CommandParameter等需要Window类型时自动触发
            /// </summary>
            public static implicit operator Window(WindowProviderImpl provider)
            {
                return provider.GetWindow();
            }
        }
    }
}