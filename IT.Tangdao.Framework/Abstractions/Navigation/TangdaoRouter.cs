using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.EventArg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Mvvm;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Abstractions.Notices;
using IT.Tangdao.Framework.Common;
using System.Runtime.Remoting;

namespace IT.Tangdao.Framework.Abstractions.Navigation
{
    /// <inheritdoc/>
    public class TangdaoRouter : ITangdaoRouter, INotifyPropertyChanged
    {
        private readonly Dictionary<string, RegistrationTypeEntry> _routeRegistry = new Dictionary<string, RegistrationTypeEntry>();
        private readonly Dictionary<Type, string> _reverseRouteMap = new Dictionary<Type, string>();  // 反向映射：类型 -> 路由名
        private readonly Stack<NavigationRecord> _backStack = new Stack<NavigationRecord>();
        private readonly Stack<NavigationRecord> _forwardStack = new Stack<NavigationRecord>();
        private readonly ITangdaoRouterResolver _routeResolver;

        private ITangdaoPage _currentPage;
        private object _currentView;

        /// <summary>
        /// 使用默认路由解析器初始化导航器实例
        /// </summary>
        public TangdaoRouter() : this(new TangdaoRouterResolver())
        { }

        /// <summary>
        /// 使用指定的路由解析器初始化导航器实例
        /// </summary>
        /// <param name="routeResolver">路由解析器实例</param>
        /// <exception cref="ArgumentNullException">当routeResolver为null时抛出</exception>
        public TangdaoRouter(ITangdaoRouterResolver routeResolver)
        {
            _routeResolver = routeResolver ?? throw new ArgumentNullException(nameof(routeResolver));
        }

        /// <inheritdoc/>
        public bool CanGoBack => _backStack.Count > 0;

        /// <inheritdoc/>
        public bool CanGoForward => _forwardStack.Count > 0;

        /// <inheritdoc/>
        public ITangdaoPage CurrentPage => _currentPage;

        /// <inheritdoc/>
        public object CurrentView
        {
            get => _currentView;
            private set
            {
                if (_currentView == value) return;
                _currentView = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public event EventHandler<RouteChangedEventArgs> RouteChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 注册页面类型（使用类型名作为路由名）
        /// </summary>
        /// <typeparam name="TPage">页面类型</typeparam>
        public void RegisterPage<TPage>() where TPage : class, ITangdaoPage
        {
            RegisterPage<TPage>(typeof(TPage).Name);
        }

        /// <summary>
        /// 使用指定路由名称注册页面类型
        /// </summary>
        /// <typeparam name="TPage">页面类型</typeparam>
        /// <param name="routeName">路由名称</param>
        public void RegisterPage<TPage>(string routeName) where TPage : class, ITangdaoPage
        {
            if (string.IsNullOrWhiteSpace(routeName))
                throw new ArgumentException("Route name cannot be null or empty", nameof(routeName));

            if (_routeRegistry.ContainsKey(routeName))
                throw new InvalidOperationException($"Route '{routeName}' is already registered");
            var pageType = typeof(TPage);
            var typeEntry = new RegistrationTypeEntry(routeName, pageType);

            _routeRegistry[routeName] = typeEntry;
            _reverseRouteMap[pageType] = routeName;
        }

        /// <summary>
        /// 导航到指定页面类型
        /// </summary>
        /// <typeparam name="TPage">页面类型</typeparam>
        /// <param name="parameters">导航参数</param>
        public void NavigateTo<TPage>(ITangdaoParameter parameters = null) where TPage : class, ITangdaoPage
        {
            var routeName = typeof(TPage).Name;
            NavigateTo(routeName, parameters);
        }

        /// <summary>
        /// 导航到指定路由名称
        /// </summary>
        /// <param name="routeName">路由名称</param>
        /// <param name="parameters">导航参数</param>
        public void NavigateTo(string routeName, ITangdaoParameter parameters = null)
        {
            if (string.IsNullOrWhiteSpace(routeName))
                throw new ArgumentException("Route name cannot be null or empty", nameof(routeName));

            // 检查当前页面是否可以离开
            if (_currentPage?.CanNavigateAway() == false)
                return;

            // 获取路由注册信息
            if (!_routeRegistry.TryGetValue(routeName, out var typeEntry))
                throw new ArgumentException($"Route '{routeName}' is not registered");

            // 解析页面实例
            var newPage = _routeResolver.ResolvePage(typeEntry);
            if (newPage == null)
                throw new InvalidOperationException($"Failed to resolve page for route '{routeName}'");

            PerformNavigation(newPage, routeName, parameters);
        }

        /// <summary>
        /// 执行导航操作
        /// </summary>
        private void PerformNavigation(ITangdaoPage newPage, string routeName, ITangdaoParameter parameters)
        {
            var previousPage = _currentPage;
            var previousRoute = GetRouteForPage(previousPage);

            if (previousPage != null)
            {
                previousPage.OnNavigatedFrom();

                // 记录到后退栈
                if (!string.IsNullOrEmpty(previousRoute))
                {
                    _backStack.Push(new NavigationRecord(previousRoute, previousPage.GetType(), previousPage, parameters));
                }
            }

            // 更新当前页面
            _currentPage = newPage;
            CurrentView = ResolveViewForPage(newPage);

            // 进入新页面
            newPage.OnNavigatedTo(parameters);

            // 清除前进栈（新导航会使前进历史无效）
            _forwardStack.Clear();

            // 触发事件
            RouteChanged?.Invoke(this, new RouteChangedEventArgs(previousPage, newPage, parameters));
            OnNavigationStateChanged();
        }

        /// <summary>
        /// 返回上一页
        /// </summary>
        public void GoBack()
        {
            if (_backStack.Count == 0) return;
            if (_currentPage?.CanNavigateAway() == false) return;

            var record = _backStack.Pop();
            var currentRoute = GetRouteForPage(_currentPage);

            var currentPage = _currentPage;

            // 记录当前页面到前进栈
            if (!string.IsNullOrEmpty(currentRoute))
            {
                _forwardStack.Push(new NavigationRecord(currentRoute, currentPage.GetType(), currentPage, null));
            }

            // 离开当前页面
            currentPage.OnNavigatedFrom();

            // 使用缓存的页面实例进行导航
            _currentPage = record.PageInstance;
            CurrentView = ResolveViewForPage(record.PageInstance);

            // 进入新页面
            record.PageInstance.OnNavigatedTo(record.Parameters);

            // 触发事件
            RouteChanged?.Invoke(this, new RouteChangedEventArgs(currentPage, record.PageInstance, record.Parameters));
            OnNavigationStateChanged();
        }

        /// <summary>
        /// 前进到下一页
        /// </summary>
        public void GoForward()
        {
            if (_forwardStack.Count == 0) return;
            if (_currentPage?.CanNavigateAway() == false) return;

            var record = _forwardStack.Pop();
            var currentRoute = GetRouteForPage(_currentPage);
            var currentPage = _currentPage;

            // 记录当前页面到后退栈
            if (!string.IsNullOrEmpty(currentRoute))
            {
                _backStack.Push(new NavigationRecord(currentRoute, currentPage.GetType(), currentPage, null));
            }

            // 离开当前页面
            currentPage.OnNavigatedFrom();

            // 使用缓存的页面实例进行导航
            _currentPage = record.PageInstance;
            CurrentView = ResolveViewForPage(record.PageInstance);

            // 进入新页面
            record.PageInstance.OnNavigatedTo(record.Parameters);

            // 触发事件
            RouteChanged?.Invoke(this, new RouteChangedEventArgs(currentPage, record.PageInstance, record.Parameters));
            OnNavigationStateChanged();
        }

        /// <summary>
        /// 获取页面对应的路由名称
        /// </summary>
        private string GetRouteForPage(ITangdaoPage page)
        {
            if (page == null) return null;

            var pageType = page.GetType();
            return _reverseRouteMap.TryGetValue(pageType, out var route) ? route : null;
        }

        /// <summary>
        /// 解析页面对应的视图
        /// </summary>
        private static object ResolveViewForPage(ITangdaoPage page)
        {
            return ViewToViewModelLocator.Build(page);
        }

        /// <summary>
        /// 触发导航状态变化通知
        /// </summary>
        private void OnNavigationStateChanged()
        {
            OnPropertyChanged(nameof(CanGoBack));
            OnPropertyChanged(nameof(CanGoForward));
        }

        /// <summary>
        /// 触发属性变化通知
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 导航记录
        /// </summary>
        private sealed class NavigationRecord
        {
            public string Route { get; }
            public Type PageType { get; }
            public ITangdaoParameter Parameters { get; }
            public ITangdaoPage PageInstance { get; }

            public NavigationRecord(string route, Type pageType, ITangdaoPage pageInstance, ITangdaoParameter parameters)
            {
                Route = route;
                PageType = pageType;
                PageInstance = pageInstance;
                Parameters = parameters;
            }
        }
    }
}