using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IT.Tangdao.Framework.Helpers
{
    public static class WindowHelper
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
    }
}
