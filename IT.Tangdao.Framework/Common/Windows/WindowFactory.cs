using IT.Tangdao.Framework.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.Common.Windows
{
    public static class WindowFactory
    {
        private static Dictionary<Type, Type> _viewModelMap;

        // 启动时调用一次，扫描映射关系
        public static void Initialize()
        {
            _viewModelMap = new Dictionary<Type, Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                foreach (var type in asm.GetTypes())
                {
                    // 扫描所有 Window 派生类
                    if (type.IsSubclassOf(typeof(Window)))
                    {
                        // 约定：LoginView → LoginViewModel
                        var vmTypeName = type.FullName.Replace("View", "ViewModel");
                        var vmType = asm.GetType(vmTypeName);
                        if (vmType != null && typeof(IGuardAware).IsAssignableFrom(vmType))
                        {
                            _viewModelMap[type] = vmType;
                        }
                    }
                }
            }
        }

        // 创建窗口实例（直接反射构造）
        public static Window CreateWindow(Type windowType)
        {
            var constructor = windowType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new InvalidOperationException($"窗口 {windowType.Name} 必须有无参构造函数");

            return constructor.Invoke(null) as Window;
        }

        // 绑定 ViewModel
        public static void BindViewModel(Window window)
        {
            var windowType = window.GetType();
            if (_viewModelMap.TryGetValue(windowType, out var vmType))
            {
                var vm = Activator.CreateInstance(vmType);
                window.DataContext = vm;
            }
        }
    }
}