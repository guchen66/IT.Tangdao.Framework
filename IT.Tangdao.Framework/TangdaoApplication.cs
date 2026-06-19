using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Mvvm;
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
using IT.Tangdao.Framework.Infrastructure;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// WPF 专用启动入口。
    /// </summary>
    [AssemblyScan]
    [Component]
    public class TangdaoApplication : TangdaoApplicationBase
    {
        protected static ITangdaoProvider Provider { get; private set; }

        //框架测试阶段，先保留日志记录
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(TangdaoApplication));

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //初始化容器
            InitializeContainer();
            //仅在此处Build
            var builder = TangdaoContainerBuilder.Current;

            //初始化模块
            InitializeModules(builder);

            //初始化解析器
            Provider = builder.Container.BuildProvider();

            //初始化服务定位器
            ServiceLocator.Default.Initialize(Provider);

            //回调插件的Initialized
            builder.RaiseBuilt(Provider);

            // 子类做额外配置
            Configure();

            //启动异步任务流
            AsyncTaskHandler(Provider.GetService<ITaskQueueManager>()).ConfigureAwait(false);

            // 创建主窗口
            InitializeWindow();
        }

        /// <summary>
        /// 设置获取容器
        /// </summary>
        private void InitializeContainer()
        {
            TangdaoContainerBuilder.SetContainerExtension(CreateContainer);
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        private void InitializeModules(TangdaoContainerBuilder builder)
        {
            //注册服务
            RegisterServices(builder.Container);
            //发现Module模块
            var moduleCatalog = ModuleFinder.SelectModules();
            //注册模块+模块回调
            ModuleFinder.RegisterModules(moduleCatalog, builder);
            //防御性编程，运行时检测未注册的服务
            builder.ValidateDependencies();
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
            return WindowFinder.GetWindow(this);
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
        private static Window CreateShell(Type shellType)
        {
            var shell = (Window)Provider.GetService(shellType) ?? throw new InvalidOperationException($"主窗口 {shellType.Name} 未注册。");
            ViewToViewModelLocator.FindAndBindViewModel(shell);
            return shell;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            (Provider as IDisposable)?.Dispose();
            base.OnExit(e);
        }
    }
}