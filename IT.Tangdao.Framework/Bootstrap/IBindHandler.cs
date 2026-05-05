using IT.Tangdao.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IT.Tangdao.Framework.Common.Reflection;
using IT.Tangdao.Framework.Extensions;

namespace IT.Tangdao.Framework.Bootstrap
{
    /// <summary>
    /// 职责单一，只处理Window和UserControl的绑定服务
    /// </summary>
    public interface IBindHandler
    {
        void AutoBindViewModel(DependencyObject view, Type viewType);
    }

    public class BindHandler : IBindHandler
    {
        public void AutoBindViewModel(DependencyObject view, Type viewType)
        {
            ViewToViewModelLocator.AutoBindViewModel(view, viewType);
        }
    }

    public static class ViewModelBinder
    {
        private static readonly Dictionary<Type, Type> _viewToViewModelMap = new Dictionary<Type, Type>();

        /// <summary>
        /// 初始化 View 与 ViewModel 的映射关系（可放在程序启动时调用一次）
        /// </summary>
        public static void InitializeMappings()
        {
            var assemblies = ReflectionServerContextExtension.assemblies;

            // 1. 找出所有可能是 View 的类型：Window、UserControl、ContentControl
            var viewTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && (
                    typeof(Window).IsAssignableFrom(t) ||
                    typeof(UserControl).IsAssignableFrom(t)
                ))
                .ToList();

            // 2. 找出所有可能是 ViewModel 的类型：命名以 ViewModel 结尾
            var viewModelTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("ViewModel"))
                .ToList();

            // 3. 试图为每个 View 找到对应的 ViewModel
            foreach (var viewType in viewTypes)
            {
                var viewTypeName = viewType.Name;

                // 约定：XXXWindow -> XXXWindowViewModel 或 XXXView -> XXXViewModel
                string expectedVmName = viewTypeName.Replace("Window", "ViewModel")
                                                    .Replace("View", "ViewModel");

                var viewModelType = viewModelTypes.FirstOrDefault(t => t.Name == expectedVmName);

                if (viewModelType != null)
                {
                    _viewToViewModelMap[viewType] = viewModelType;
                }
            }
        }

        /// <summary>
        /// 根据 View 的类型获取对应的 ViewModel 类型
        /// </summary>
        public static Type GetViewModelType(Type viewType)
        {
            if (_viewToViewModelMap.TryGetValue(viewType, out var vmType))
                return vmType;

            return null;
        }

        /// <summary>
        /// 自动绑定 ViewModel 到 View（设置 DataContext）
        /// </summary>
        public static void AutoBindViewModel(Window view)
        {
            var viewModelType = GetViewModelType(view.GetType());
            if (viewModelType == null)
                return; // 或者抛异常，根据需求

            var viewModel = Activator.CreateInstance(viewModelType);
            view.DataContext = viewModel;
        }
    }
}