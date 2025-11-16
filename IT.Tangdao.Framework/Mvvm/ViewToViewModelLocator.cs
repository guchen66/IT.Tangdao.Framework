using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.DaoException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Abstractions.Navigates;
using System.Reflection;
using IT.Tangdao.Framework.Threading;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Abstractions.Loggers;
using System.Threading;
using IT.Tangdao.Framework.Abstractions.Contracts;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace IT.Tangdao.Framework.Mvvm
{
    /// <summary>
    /// 视图定位器
    /// 用来全局定位MVVM模式
    /// </summary>
    internal static class ViewToViewModelLocator
    {
        /// <summary>
        /// 针对普通 VM，零约束
        /// </summary>
        public static object Build(object vm) => BuildInstance(vm);

        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(ViewToViewModelLocator));

        /// <summary>
        /// 针对实现了 ITangdaoPage 的 VM，编译期就能确定类型，零装箱
        /// </summary>
        public static object Build<T>(T vm) where T : ITangdaoPage => BuildInstance(vm);

        /// <summary>
        /// 先尝试在同一程序集中查找，如果使用 Type.GetType(viewTypeName)会报错，因为dll不存在你程序的命名空间，
        /// 此时为null，你可以设置数据模板
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        private static object BuildInstance(object vm)
        {
            var vmType = vm.GetType();

            var viewTypeName = vmType.FullName
                .Replace("ViewModel", "View")
                .Replace("ViewModels", "Views");

            var viewType = vmType.Assembly.GetType(viewTypeName);
            if (viewType == null) return vm;

            var view = Activator.CreateInstance(viewType);

            if (view is FrameworkElement fe)
                fe.DataContext = vm;

            return view;
        }

        /// <summary>
        /// 自动为 View 绑定对应的 ViewModel
        /// </summary>
        public static void AutoBindViewModel(DependencyObject view, Type viewType, ITangdaoProvider Provider)
        {
            // 查找对应的 ViewModel 类型
            var viewModelType = FindViewModelType(viewType);

            if (viewModelType != null)
            {
                // 从容器解析 ViewModel 并设置 DataContext
                var viewModel = Provider.GetService(viewModelType);
                if (viewModel != null)
                {
                    if (view is FrameworkElement frameworkElement)
                    {
                        frameworkElement.DataContext = viewModel;
                        if (viewModel is IViewReady life)
                        {
                            // 显式委托变量，用于解绑
                            RoutedEventHandler handler = null;
                            handler = (s, e) =>
                            {
                                frameworkElement.Loaded -= handler;   // 立刻解绑
                                life.OnViewLoaded();
                            };
                            frameworkElement.Loaded += handler;
                            if (frameworkElement.IsLoaded)
                                handler(frameworkElement, new RoutedEventArgs());
                        }
                    }
                }
            }
            RegisterAutoViews();
        }

        /// <summary>
        /// 根据 View 类型查找对应的 ViewModel 类型
        /// </summary>
        private static Type FindViewModelType(Type viewType)
        {
            if (viewType == null) return null;

            var viewModelName = viewType.Name.Replace("View", "ViewModel");

            // 方法1：同命名空间
            var viewModelType = viewType.Assembly.GetType($"{viewType.Namespace}.{viewModelName}");
            if (viewModelType != null) return viewModelType;

            // 方法2：全局搜索（最简单可靠）
            viewModelType = viewType.Assembly.GetTypes()
                .FirstOrDefault(t => t.Name == viewModelName);

            if (viewModelType != null)
            {
                return viewModelType;
            }

            return null;
        }

        private static readonly Assembly _entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

        private static int _registered; // 0 未注册，1 已注册

        public static void RegisterAutoViews()
        {
            if (_entryAssembly == null)
            {
                return;
            }
            if (Interlocked.Exchange(ref _registered, 1) == 1) return; // 原子幂等

            var vms = _entryAssembly.GetTypes()
                .Where(t => !t.IsAbstract &&
                            Attribute.IsDefined(t, typeof(AutoWireViewAttribute)));

            foreach (var vmType in vms)
            {
                var key = new DataTemplateKey(vmType);
                if (Application.Current.Resources.Contains(key)) continue;
                var viewTypeName = vmType.FullName.Replace("ViewModel", "View").Replace("ViewModels", "Views");
                var viewType = vmType.Assembly.GetType(viewTypeName);

                var factory = new FrameworkElementFactory(viewType);
                factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnViewLoaded));
                var dataTemplate = new DataTemplate { DataType = vmType, VisualTree = factory };
                Application.Current.Resources[key] = dataTemplate;
            }
        }

        private static void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            // sender 就是「模板实例」本身
            var view = (FrameworkElement)sender;
            // 解除订阅，只执行一次
            view.Loaded -= OnViewLoaded;

            var vm = view.DataContext;          // 已由 ContentPresenter 自动灌入
            if (vm is IViewReady life)
                life.OnViewLoaded();
        }
    }
}