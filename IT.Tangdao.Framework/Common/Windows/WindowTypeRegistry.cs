using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.Common.Windows
{
    /// <summary>
    ///  启动时反射收集所有窗口类型（一次性）
    /// </summary>
    public static class WindowTypeRegistry
    {
        // 只需要一个"万能委托"
        private static Func<Type, Window> _windowFactory = (windowType) =>
        {
            // 现场解析、创造窗口实例
            // 而不是从字典里查找预注册的委托
            return ServiceLocator.Default.GetService(windowType) as Window;
        };

        public static Window CreateWindow(Type windowType)
        {
            return _windowFactory(windowType);
        }
    }
}