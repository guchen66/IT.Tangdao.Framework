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
using System.Windows.Controls;
using System.Windows.Media;
using IT.Tangdao.Framework.Ioc;

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
        public static ITangdaoProvider Provider { get; private set; } = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ① 仅在此处Build
            var builder = new TangdaoContainerBuilder();
            RegisterServices(builder.Container);   // 暴露 Container 仅此时有效
            builder.ValidateDependencies();
            var container = builder.Build();
            Provider = container.BuildProvider();

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
        /// 约定装配主窗口：
        /// 1. 解析 TShell；
        /// 2. 扫描所有 [InjectContent] 标记的 ContentControl；
        /// 3. 按约定加载 UserControl + ViewModel 并注入。
        /// </summary>
        /// <summary>
        /// 类型安全入口：子类可主动调用，也可被框架自动调用。
        /// </summary>
        protected Window CreateShell<TShell>() where TShell : Window
            => CreateShell(typeof(TShell));

        /// <summary>
        /// 内部实现：按约定装配主窗口（非泛型，零反射重复）。
        /// </summary>
        /// <summary>
        /// 创建主窗口并自动绑定 ViewModel
        /// </summary>
        protected Window CreateShell(Type shellType)
        {
            // 解析窗口实例
            var shell = (Window)Provider.GetService(shellType)
                     ?? throw new InvalidOperationException($"主窗口 {shellType.Name} 未注册。");

            // 自动绑定窗口的 ViewModel
            AutoBindViewModel(shell, shellType);

            // 处理 ContentControl 的内容注入
            // InjectContentControls(shell);

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
        /// 注入标记了 特性 的 ViewModel
        /// </summary>
        /// <summary>
        /// 按需注入：只给当前窗口里「空着」的 ContentControl 装内容。
        /// 约定：ContentControl 名称去掉后缀后对应 View 类型。
        /// </summary>
        private static void InjectContentControls(DependencyObject parent)
        {
            // ① 递归扫视觉树
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // ② 如果是 ContentControl 且现在没内容
                if (child is ContentControl cc && cc.Content == null)
                {
                    // ③ 用约定拿到 View 类型
                    var viewType = GuessViewType(cc.GetType());
                    if (viewType == null) continue;

                    // ④ 容器解析 View
                    var view = Provider.GetService(viewType) as Control;
                    if (view == null) continue;

                    // ⑤ 再给它绑 ViewModel
                    var vmType = GuessViewModelType(viewType);
                    if (vmType != null)
                        view.DataContext = Provider.GetService(vmType);

                    cc.Content = view;
                }

                // 继续往下走
                InjectContentControls(child);
            }
        }

        /// <summary>
        /// 最简单的命名约定：ContentControl 叫 xxxNavigationPanel，
        /// 对应 View 就叫 NavigationPanelView。
        /// </summary>
        private static Type GuessViewType(Type controlType)
        {
            var name = controlType.Name
                                  .Replace("ContentControl", "")
                                  .Replace("Control", "") + "View";
            return controlType.Assembly.GetType($"{controlType.Namespace}.{name}");
        }

        /// <summary>
        /// View → ViewModel 的约定，和你原来 FindViewModelType 逻辑一致
        /// </summary>
        private static Type GuessViewModelType(Type viewType)
        {
            var vmName = viewType.Name.Replace("View", "") + "ViewModel";
            return viewType.Assembly.GetTypes()
                           .FirstOrDefault(t => t.Name == vmName);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            (Provider as IDisposable)?.Dispose();
            base.OnExit(e);
        }
    }
}