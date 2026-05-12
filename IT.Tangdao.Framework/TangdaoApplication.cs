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
using IT.Tangdao.Framework.DaoTasks;
using System.Windows.Forms;
using IT.Tangdao.Framework.Windows;
using IT.Tangdao.Framework.Common.Windows;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// WPF 专用启动入口。
    /// </summary>
    [AssemblyScan]
    public class TangdaoApplication : TangdaoApplicationBase
    {
        protected static ITangdaoProvider Provider { get; private set; }

        //框架测试阶段，先保留日志记录
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(TangdaoApplication));

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeInternal();
            // ① 仅在此处Build
            var builder = TangdaoContainerBuilder.Current;
            RegisterServices(builder.Container);   // 暴露 Container 仅此时有效

            //发现Module模块，注册模块
            var moduleCatalog = DiscoverModules();
            RegisterModules(moduleCatalog, builder);
            builder.ValidateDependencies();
            Provider = builder.Build().BuildProvider();

            //服务定位器初始化
            ServiceLocator.Default.Initialize(Provider);
            builder.RaiseBuilt(Provider);     //回调插件的Initialized

            // ② 留给子类做额外配置
            Configure();
            AsyncTaskHandler(Provider.GetService<ITaskQueueManager>()).ConfigureAwait(false);
            // ③ 创建主窗口
            InitializeWindow();
        }

        /// <summary>
        /// 设置获取容器
        /// </summary>
        private void InitializeInternal()
        {
            TangdaoContainerBuilder.SetContainerExtension(CreateContainer);
        }

        private void InitializeWindow()
        {
            var window = CreateWindow();
            // ② 摆烂时走约定
            if (window == null)
                window = CreateShell(GetMainWindowType());
            if (window != null)
            {
                SetMainWindow(window);
            }
            ConfigureWindow();
            ShowShell();
        }

        private void ConfigureWindow()
        {
            ITangdaoProvider _provider = Provider;
            var build = _provider.GetService<IWindowBuilder>();
            ConfigureWindowPipe(build);
            build.ExecuteAll();
        }

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
        private Type GetMainWindowType()
        {
            var assembly = GetType().Assembly;
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

        /// <summary>
        /// 设置 Application.MainWindow（不显示）
        /// </summary>
        private void SetMainWindow(Window shell)
        {
            if (shell == null)
            {
                throw new InvalidOperationException("无法创建主窗口，请重写 CreateWindow() 方法返回您的主窗口。");
            }

            base.MainWindow = shell;
        }

        /// <summary>
        /// 显示Shell
        /// </summary>
        private void ShowShell()
        {
            if (base.MainWindow != null)
            {
                base.MainWindow.Show();
            }
        }

        /// <summary>
        /// 类型安全入口：子类可主动调用，也可被框架自动调用。
        /// </summary>
        protected static Window CreateShell<TShell>() where TShell : Window
            => CreateShell(typeof(TShell));

        /// <summary>
        /// 创建主窗口并自动绑定 ViewModel
        /// </summary>
        protected static Window CreateShell(Type shellType)
        {
            var shell = (Window)Provider.GetService(shellType)
                   ?? throw new InvalidOperationException($"主窗口 {shellType.Name} 未注册。");

            ViewToViewModelLocator.FindAndBindViewModel(shell);
            return shell;
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