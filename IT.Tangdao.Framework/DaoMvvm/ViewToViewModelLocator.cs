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

namespace IT.Tangdao.Framework.DaoMvvm
{
    /// <summary>
    /// 视图定位器
    /// 用来全局定位MVVM模式
    /// </summary>
    public class ViewToViewModelLocator
    {
        public static ITangdaoContainer Build(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var name = type.FullName.Replace("ViewModel", "View");

                var viewType = Type.GetType(name);
                // viewType.get();
                if (viewType.IsSubclassOf(typeof(UserControl)))
                {
                    var view = (UserControl)Activator.CreateInstance(viewType);
                    view.DataContext = viewType;
                }
                else if (viewType.IsSubclassOf(typeof(Window)))
                {
                    var view = (Window)Activator.CreateInstance(viewType);
                    view.DataContext = viewType;
                }
                //var view = (Window)Activator.CreateInstance(viewType);
                var propertyInfo = viewType.GetProperty("DataContext");
            }

            return new TangdaoContainer();
        }

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

        private static readonly Assembly _assembly = Assembly.GetEntryAssembly();

        /// <summary>
        /// 根据 ViewModel 类型自动关联 View 和 ViewModel。
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel 类型。</typeparam>
        /// <returns>关联后的 View 实例。</returns>
        public static Window ResolveView<TViewModel>() where TViewModel : class
        {
            // 获取 ViewModel 类型
            var viewModelType = typeof(TViewModel);

            // 查找 ViewToViewModelAttribute
            var attribute = viewModelType.GetCustomAttribute<ViewToViewModelAttribute>();
            if (attribute == null)
            {
                throw new InvalidOperationException($"ViewModel {viewModelType.Name} 未标记 ViewToViewModelAttribute。");
            }

            // 获取 View 类型
            var viewType = _assembly.GetTypes()
                .FirstOrDefault(t => t.Name == attribute.ViewName);
            if (viewType == null)
            {
                throw new InvalidOperationException($"未找到名为 {attribute.ViewName} 的 View。");
            }

            // 创建 View 和 ViewModel 实例
            var view = Activator.CreateInstance(viewType) as Window;
            var viewModel = Activator.CreateInstance(viewModelType);

            // 设置 DataContext
            view.DataContext = viewModel;

            return view;
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
                var control = (Control)provider.Resolve(type);
                control.DataContext = data;
                return control;
            }

            return new Control();
        }
    }
}