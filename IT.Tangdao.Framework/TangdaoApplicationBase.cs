using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.DaoMvvm;
using IT.Tangdao.Framework.Selectors;
using System.Runtime.CompilerServices;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.DaoCommon;

namespace IT.Tangdao.Framework
{
    public abstract class TangdaoApplicationBase : Application
    {
        protected ITangdaoContainer Container;

        protected ITangdaoProvider Provider;

        protected TangdaoApplicationBase()
        {
            Container = TangdaoContainerBuilder.CreateContainer();
            Provider = TangdaoContainerBuilder.Builder();
            ServerLocator.InitContainer(Container);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeInternal();
        }

        private void InitializeInternal()
        {
            Register(Container);
            // 自动关联 View 和 ViewModel
            AutoWireViewAndViewModel();
            //  OnConfigure(Context);
            //RegisterBindable();
            OnInitialized();
        }

        protected abstract void Register(ITangdaoContainer Container);

        // protected abstract void RegisterBindable<TView,TViewModel>();
        /// <summary>
        /// 自动关联 View 和 ViewModel。
        /// </summary>
        //private void AutoWireViewAndViewModel()
        //{
        //    //获取当前运行的程序集
        //    Assembly assembly = Application.Current.GetType().Assembly;

        //    // 使用 ViewToViewModelExtension 扫描标记了 ViewToViewModelAttribute 的类型
        //    var viewModelTypes = TangdaoAttributeSelector.FindClassesWithViewToViewModelAttribute(assembly);
        //    var name = viewModelTypes.First().Name;
        //    Console.WriteLine(name);
        //    // System.Reflection.MemberInfo
        //    // System.Reflection.PropertyInfo
        //    // System.Reflection.ParameterInfo
        //    // RuntimeHelpers.
        //    foreach (var viewModelType in viewModelTypes)
        //    {
        //        ViewToViewModelLocator.Build(Provider.Resolve(viewModelType), Provider);
        //    }
        //}

        private void OnInitialized()
        {
            Window window = CreateWindow();
            if (window != null)
            {
                InitWindow(window);
            }
        }

        private void InitWindow(Window window)
        {
            window.Show();
        }

        protected abstract Window CreateWindow();

        public void AutoWireViewAndViewModel()
        {
            // 获取当前程序集
            Assembly assembly = Application.Current?.GetType().Assembly ??
                               Assembly.GetExecutingAssembly();

            // 获取所有标记了 ViewToViewModelAttribute 的 ViewModel 类型
            var viewModelTypes = TangdaoAttributeSelector.FindViewToViewModelAttribute(assembly);

            foreach (var viewModelType in viewModelTypes)
            {
                // 获取 ViewToViewModelAttribute
                TangdaoContext.GetContext(viewModelType);
                var attribute = viewModelType.GetCustomAttribute<ViewToViewModelAttribute>();
                if (attribute == null) continue;

                string viewName = attribute.ViewName;
                Console.WriteLine("ViewName:" + viewName);
                Type viewType = assembly.GetType(viewName) ??
                               assembly.GetTypes().FirstOrDefault(t => t.Name == viewName);

                if (viewType != null)
                {
                    WireViewAndViewModel(viewType, viewModelType);
                }
            }
        }

        private void WireViewAndViewModel(Type viewType, Type viewModelType)
        {
            // 创建 ViewModel 实例（处理构造器参数）
            object viewModelInstance = CreateViewModelInstance(viewModelType);

            // 查找或创建 View 实例
            object viewInstance = FindOrCreateViewInstance(viewType);

            // 设置 DataContext
            if (viewInstance is FrameworkElement frameworkElement)
            {
                frameworkElement.DataContext = viewModelInstance;
            }
            else if (viewInstance != null)
            {
                // 使用反射设置 DataContext 属性（对于非FrameworkElement）
                PropertyInfo dataContextProperty = viewType.GetProperty("DataContext");
                dataContextProperty?.SetValue(viewInstance, viewModelInstance);
            }
        }

        private object CreateViewModelInstance(Type viewModelType)
        {
            // 获取所有构造器
            var constructors = viewModelType.GetConstructors();

            // 优先选择参数最少的构造器（通常是无参构造器）
            var constructor = constructors.OrderBy(c => c.GetParameters().Length).FirstOrDefault();

            if (constructor == null)
            {
                // 如果没有公共构造器，尝试使用 FormatterServices 绕过构造器
                return CreateInstanceWithoutConstructor(viewModelType);
            }

            // 解析构造器参数
            var parameters = constructor.GetParameters();
            object[] parameterValues = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                Provider.Resolve(parameters[i].ParameterType);
                //  parameterValues[i] = Provider.Resolve(parameters[i].ParameterType);
            }

            // 使用构造器创建实例
            return constructor.Invoke(parameterValues);
        }

        private static object CreateInstanceWithoutConstructor(Type type)
        {
            try
            {
                // 使用 FormatterServices 绕过构造器
                return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
            }
            catch
            {
                // 如果失败，尝试使用 Activator（可能会调用默认构造器）
                return Activator.CreateInstance(type);
            }
        }

        private static object FindOrCreateViewInstance(Type viewType)
        {
            // 首先尝试在 Application.Current.Windows 中查找现有的 View 实例
            if (Application.Current != null)
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == viewType)
                    {
                        return window;
                    }
                }
            }

            // 如果没有找到现有的实例，创建新的 View 实例
            try
            {
                return Activator.CreateInstance(viewType);
            }
            catch
            {
                // 如果创建失败，返回 null（View 可能需要在特定上下文中创建）
                return null;
            }
        }
    }
}