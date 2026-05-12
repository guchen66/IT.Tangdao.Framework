using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Threading;
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
        private Type _windowType;
        private bool _isAllowShow = false;
        private Action _onSuccess;
        private Action _onFailure;
        public ShowMode ShowMode { get; set; }
        private GuardContext _guardContext;
        private IEventAggregator _eventAggregator;

        public WindowPipeline(IWindowGuard guard, IEventAggregator eventAggregator)
        {
            _guard = guard;
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// 配置窗体类型
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <returns></returns>
        public IWindowPipeline Configure<TWindow>() where TWindow : Window
        {
            _windowType = typeof(TWindow);
            return this;
        }

        /// <summary>
        /// 设置窗体是否允许打开
        /// </summary>
        /// <param name="allow"></param>
        /// <returns></returns>
        public IWindowPipeline SetActive(bool allow)
        {
            _isAllowShow = allow;
            return this;
        }

        public void SetContext(GuardContext context)
        {
            _guardContext = context;
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
            if (_windowType == null)
                throw new InvalidOperationException("未配置窗口类型");

            var winEvent = new WinEventBase
            {
                WindowType = _windowType,
                IsShow = _isAllowShow,
                ShowMode = ShowMode,
                SucessAction = _onSuccess,
                FailureAction = _onFailure,
                Context = _guardContext
            };
            _eventAggregator.Publish(winEvent);
            return true;
        }
    }
}