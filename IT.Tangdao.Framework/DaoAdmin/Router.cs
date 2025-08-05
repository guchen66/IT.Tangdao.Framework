using IT.Tangdao.Framework.DaoEvents;
using IT.Tangdao.Framework.DaoIoc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public class Router : IRouter, INotifyPropertyChanged
    {
        private readonly Dictionary<string, Func<ITangdaoPage>> _routeMap = new Dictionary<string, Func<ITangdaoPage>>();
        private readonly Dictionary<Type, string> _reverseRouteMap = new Dictionary<Type, string>();
        private readonly Stack<NavigationRecord> _backStack = new Stack<NavigationRecord>();
        private readonly Stack<NavigationRecord> _forwardStack = new Stack<NavigationRecord>();

        public ITangdaoPage CurrentPage { get; private set; }

        public event EventHandler<RouteChangedEventArgs> RouteChanged;

        // 添加CanGoBack和CanGoForward属性
        public bool CanGoBack => _backStack.Count > 0;

        public bool CanGoForward => _forwardStack.Count > 0;

        // 修改NavigateToInternal方法
        private async Task NavigateToInternal(ITangdaoPage newPage, object parameters)
        {
            if (CurrentPage?.CanNavigateAway() == false)
                return;

            var previousPage = CurrentPage;

            if (previousPage != null)
            {
                await previousPage.OnNavigatedFromAsync();
                // 确保有路由信息才记录历史
                if (_reverseRouteMap.TryGetValue(previousPage.GetType(), out var previousRoute))
                {
                    _backStack.Push(new NavigationRecord(previousRoute, previousPage.GetType(), parameters));
                }
            }

            CurrentPage = newPage;
            CurrentView = ResolveViewForPage(newPage);

            await newPage.OnNavigatedToAsync(parameters);
            _forwardStack.Clear();

            // 通知导航状态变化
            RouteChanged?.Invoke(this, new RouteChangedEventArgs(previousPage, newPage, parameters));
            OnNavigationStateChanged();
        }

        // 添加导航状态变化通知
        private void OnNavigationStateChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanGoBack)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanGoForward)));
        }

        public void RegisterRoute(string route, Func<ITangdaoPage> pageFactory)
        {
            _routeMap[route] = pageFactory;
        }

        public void RegisterPage<T>() where T : ITangdaoPage, new()
        {
            var type = typeof(T);
            var route = type.Name;
            // 替换范围运算符为Substring
            if (route.EndsWith("Page"))
                route = route.Substring(0, route.Length - 4);
            if (route.EndsWith("ViewModel"))
                route = route.Substring(0, route.Length - 9);

            RegisterRoute(route, () => new T());
            _reverseRouteMap[type] = route;
        }

        public async void NavigateTo<T>(object parameters = null) where T : ITangdaoPage
        {
            await NavigateToInternal(typeof(T), parameters);
        }

        public async void NavigateTo(string route, object parameters = null)
        {
            if (!_routeMap.TryGetValue(route, out var pageFactory))
                throw new ArgumentException($"Route '{route}' is not registered");

            await NavigateToInternal(pageFactory(), parameters);
        }

        private async Task NavigateToInternal(Type pageType, object parameters)
        {
            if (!_reverseRouteMap.TryGetValue(pageType, out var route))
                throw new ArgumentException($"Page type {pageType.Name} is not registered");

            var pageFactory = _routeMap[route];
            await NavigateToInternal(pageFactory(), parameters);
        }

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void GoBack()
        {
            if (_backStack.Count == 0) return;

            var record = _backStack.Pop();
            _forwardStack.Push(new NavigationRecord(_reverseRouteMap[CurrentPage.GetType()], CurrentPage.GetType(), null));
            await NavigateToInternal(record.PageType, record.Parameters);
        }

        public async void GoForward()
        {
            if (_forwardStack.Count == 0) return;

            var record = _forwardStack.Pop(); _backStack.Push(new NavigationRecord(_reverseRouteMap[CurrentPage.GetType()], CurrentPage.GetType(), null));
            await NavigateToInternal(record.PageType, record.Parameters);
        }

        private object ResolveViewForPage(ITangdaoPage page)
        {
            // 简单实现：假设页面本身就是视图
            return page;

            // 或者使用约定优于配置的方式：
            // var viewTypeName = page.GetType().FullName.Replace("ViewModel", "View");
            // var viewType = Type.GetType(viewTypeName);
            // return viewType != null ? Activator.CreateInstance(viewType) : page;
        }

        private class NavigationRecord
        {
            public string Route { get; }
            public Type PageType { get; }
            public object Parameters { get; }

            public NavigationRecord(string route, Type pageType, object parameters)
            {
                Route = route;
                PageType = pageType;
                Parameters = parameters;
            }
        }
    }
}