using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using IT.Tangdao.Framework.Bootstrap;
using IT.Tangdao.Framework.Abstractions.Loggers;

namespace IT.Tangdao.Framework.Infrastructure
{
    public static class WindowFinder
    {
        public static List<TWindow> FindAllActiveChildWindows<TWindow>(TWindow mainWindow) where TWindow : Window
        {
            var activeChildWindows = new List<TWindow>();
            FindActiveChildWindows<TWindow>(mainWindow, activeChildWindows);
            return activeChildWindows;
        }

        private static void FindActiveChildWindows<TWindow>(TWindow window, List<TWindow> activeChildWindows) where TWindow : Window
        {
            foreach (Window childWindow in Application.Current.Windows)
            {
                if (childWindow.Owner == window && childWindow.IsActive)
                {
                    TWindow typedChildWindow = childWindow as TWindow;
                    if (typedChildWindow != null)
                    {
                        activeChildWindows.Add(typedChildWindow);
                        FindActiveChildWindows<TWindow>(typedChildWindow, activeChildWindows);
                    }
                }
            }
        }

        public static Window FindMainWindow(DependencyObject child)
        {
            // 获取当前元素的父元素
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            //如果当前元素没有父元素，或者父元素不是Window类型，则返回null
            if (parent == null || !(parent is Window))
                return null;

            // 检查当前父元素是否是MainWindow
            var window = parent as Window;
            if (window != null)
                return window;

            // 如果当前父元素不是MainWindow，递归调用FindMainWindow查找
            return FindMainWindow(parent);
        }

        public static Type GetWindow(ITangdaoDataProvider tangdaoDataProvider)
        {
            var assembly = tangdaoDataProvider.GetType().Assembly;
            var windowTypes = assembly.GetExportedTypes().Where(t => t.IsSubclassOf(typeof(Window))).ToList();
            if (windowTypes.Count == 0)
            {
                return null;
            }

            if (windowTypes.Count > 1)
            {
                string typeNames = string.Join(", ", windowTypes.Select(t => t.Name));
                throw new InvalidOperationException(
                    $"找到多个 Window 派生类：{typeNames}。\n" +
                    "请重写 CreateWindow() 方法明确指定要使用的主窗口。");
            }
            return windowTypes[0];
        }
    }
}