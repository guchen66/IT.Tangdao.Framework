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

namespace IT.Tangdao.Framework.Mvvm
{
    /// <summary>
    /// 视图定位器
    /// 用来全局定位MVVM模式
    /// </summary>
    public class ViewToViewModelLocator
    {
        /// <summary>
        /// 针对普通 VM，零约束
        /// </summary>
        public static object Build(object vm) => BuildInstance(vm);

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
                    }
                }
            }
        }

        /// <summary>
        /// 根据 View 类型查找对应的 ViewModel 类型
        /// </summary>
        private static Type FindViewModelType(Type viewType)
        {
            if (viewType == null) return null;

            var viewModelName = viewType.Name.Replace("View", "") + "ViewModel";

            // 方法1：同命名空间
            var viewModelType = viewType.Assembly.GetType($"{viewType.Namespace}.{viewModelName}");
            if (viewModelType != null) return viewModelType;

            // 方法2：全局搜索（最简单可靠）
            viewModelType = viewType.Assembly.GetTypes()
                .FirstOrDefault(t => t.Name == viewModelName);

            if (viewModelType != null)
            {
                Console.WriteLine($"通过全局搜索找到: {viewModelType.FullName}");
                return viewModelType;
            }

            Console.WriteLine($"未找到 ViewModel: {viewModelName}");
            return null;
        }

        /// <summary>
        /// 绑定所有的视图的DataContext
        /// </summary>
        /// <param name="data"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Control BindDataContext(object data, ITangdaoProvider provider)
        {
            if (data is null)
                return null;

            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                var control = (Control)provider.GetService(type);
                control.DataContext = data;
                return control;
            }

            return new Control();
        }
    }
}