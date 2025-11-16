using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Mvvm;
using IT.Tangdao.Framework.Helpers;
using System.Runtime.CompilerServices;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Common;
using System.Windows.Controls;
using System.Windows.Media;
using IT.Tangdao.Framework.Ioc;
using System.ComponentModel;
using IT.Tangdao.Framework.Abstractions.Loggers;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// WPF 专用启动入口。
    /// 1. OnStartup 里触发用户注册；
    /// 2. 自动解析主窗口并显示；
    /// 3. 全局 Provider 可后续解析 ViewModel、DialogService...
    /// </summary>
    public abstract class TangdaoApplication : Application
    {
        public static ITangdaoProvider Provider { get; private set; }
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(TangdaoApplication));

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ① 仅在此处Build
            var builder = new TangdaoContainerBuilder();
            RegisterServices(builder.Container);   // 暴露 Container 仅此时有效

            //发现Module模块，注册模块
            var moduleCatalog = DiscoverModules();
            RegisterModules(moduleCatalog, builder);
            builder.ValidateDependencies();
            Provider = builder.Build().BuildProvider();
            builder.RaiseBuilt(Provider);     //回调插件的Initialized

            // ② 留给子类做额外配置
            Configure();
            // ③ 创建主窗口
            var window = CreateWindow();
            // ② 摆烂时走约定
            if (window == null)
                window = CreateShell(GetMainWindowType());

            if (window != null)
            {
                MainWindow = window;
                window.Show();
            }
        }

        /// 仅在此方法内使用 Container，不要持有字段引用
        protected abstract void RegisterServices(ITangdaoContainer container);

        protected virtual void Configure()
        { }

        /// <summary>
        /// 子类**可重写**。
        /// 返回 null 表示“我不想手写，用约定”。
        /// </summary>
        protected virtual Window CreateWindow() => null;

        /// <summary>
        /// 约定：告诉框架主窗口类型。
        /// 默认实现：扫描当前入口程序集里 *唯一* 的 Window 派生类；
        /// 若不想扫描，子类**重写**此方法来指定。
        /// </summary>
        protected virtual Type GetMainWindowType()
        {
            var winType = GetType().Assembly.GetExportedTypes()
                                  .SingleOrDefault(t => t.IsSubclassOf(typeof(Window)));
            return winType ?? throw new InvalidOperationException("未找到任何 Window 派生类，请重写 GetMainWindowType()。");
        }

        /// <summary>
        /// 类型安全入口：子类可主动调用，也可被框架自动调用。
        /// </summary>
        protected static Window CreateShell<TShell>() where TShell : Window
            => CreateShell(typeof(TShell));

        /// <summary>
        /// 内部实现：按约定装配主窗口（非泛型，零反射重复）。
        /// </summary>
        /// <summary>
        /// 创建主窗口并自动绑定 ViewModel
        /// </summary>
        protected static Window CreateShell(Type shellType)
        {
            // 解析窗口实例
            var shell = (Window)Provider.GetService(shellType)
                     ?? throw new InvalidOperationException($"主窗口 {shellType.Name} 未注册。");

            // 自动绑定窗口的 ViewModel
            AutoBindViewModel(shell, shellType);
            return shell;
        }

        /// <summary>
        /// 自动为 View 绑定对应的 ViewModel
        /// </summary>
        private static void AutoBindViewModel(DependencyObject view, Type viewType)
        {
            ViewToViewModelLocator.AutoBindViewModel(view, viewType, Provider);
        }

        /// <summary>
        /// 要想生效，用户代码需要写[assembly: TangdaoModule(typeof(DemoModule))]
        /// </summary>
        /// <returns></returns>
        private static List<ITangdaoModule> DiscoverModules()
        {
            var list = new List<ITangdaoModule>();
            foreach (var asm in AssemblyHelper.GetModuleAssemblies())
            {
                foreach (var attr in asm.GetCustomAttributes<TangdaoModuleAttribute>())
                {
                    if (Activator.CreateInstance(attr.ModuleType) is ITangdaoModule module)
                        list.Add(module);
                }
            }
            return list;
        }

        /// <summary>
        /// 注册模块以及将初始化回调存在builder的委托
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="builder"></param>
        private static void RegisterModules(IReadOnlyList<ITangdaoModule> catalog, TangdaoContainerBuilder builder)
        {
            var eager = catalog.Where(m => !m.Lazy).OrderBy(m => m.Order);
            foreach (var m in eager)
            {
                m.RegisterServices(builder.Container);
                builder.AddBuiltCallback(provider => m.OnInitialized(provider));
            }

            // 懒加载模块：注册一个工厂，第一次解析时触发真实 RegisterServices
            // 延迟注册（只攒动作，不解析）
            foreach (var m in catalog.Where(m => m.Lazy))
            {
                var moduleCopy = m;
                builder.Container.AddLazyRegistration(c =>
                {
                    moduleCopy.RegisterServices(c);
                    builder.AddBuiltCallback(provider => m.OnInitialized(provider));
                });
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            (Provider as IDisposable)?.Dispose();
            base.OnExit(e);
        }
    }
}