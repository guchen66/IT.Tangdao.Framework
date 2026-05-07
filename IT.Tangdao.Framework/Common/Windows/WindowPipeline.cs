using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.Windows
{
    /// <summary>
    /// 窗口管道实现（内部类，对用户隐藏）
    /// </summary>
    internal class WindowPipeline : IWindowPipeline
    {
        public IWindowGuard Guard
        {
            get => _guard;
            set => _guard = value;
        }

        private IWindowGuard _guard;
        private Type _loginWindowType;
        private bool _allowCancel = true;
        private Action _onSuccess;
        private Action _onFailure;
        public ShowMode ShowMode { get; set; }

        private IEventAggregator _eventAggregator;

        public WindowPipeline(IWindowGuard guard, IEventAggregator eventAggregator)
        {
            _guard = guard;
            _eventAggregator = eventAggregator;
        }

        public IWindowPipeline UseLogin<TWindow>() where TWindow : Window
        {
            _loginWindowType = typeof(TWindow);
            return this;
        }

        public IWindowPipeline UseConfigure<TWindow>() where TWindow : Window
        {
            _loginWindowType = typeof(TWindow);
            return this;
        }

        public IWindowPipeline SetCancel(bool allow)
        {
            _allowCancel = allow;
            return this;
        }

        public IWindowPipeline OnSuccess(Action action)
        {
            _onSuccess = action;
            return this;
        }

        public IWindowPipeline OnFailure(Action action)
        {
            _onFailure = action;
            return this;
        }

        /// <summary>
        /// 框架内部执行：显示窗口、判断结果、调用回调
        /// </summary>
        public bool Execute()
        {
            // 1. 守卫预验证
            if (!_guard.Validate())
            {
                _onFailure?.Invoke();
                return false;
            }

            // 2. 创建并配置窗口
            if (_loginWindowType == null)
                throw new InvalidOperationException("未配置登录窗口类型");
            var window = ServiceLocator.Default.GetService(_loginWindowType) as Window;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (!_allowCancel)
            {
                window.Closing += (s, e) =>
                {
                    if (window.DialogResult == null)
                        e.Cancel = true;
                };
            }

            WinEventBase winEventBase = new WinEventBase();
            winEventBase.WindowType = _loginWindowType;
            _eventAggregator.Publish(winEventBase);

            return true;
        }
    }
}