using IT.Tangdao.Framework.DaoEvents;
using IT.Tangdao.Framework.DaoIoc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.DaoAdmin.Navigates
{
    public class TangdaoRouter : ITangdaoRouter, INotifyPropertyChanged
    {
        private readonly Dictionary<string, Func<ITangdaoPage>> _routeMap = new Dictionary<string, Func<ITangdaoPage>>();
        private readonly Dictionary<Type, string> _reverseRouteMap = new Dictionary<Type, string>();
        private readonly Stack<NavigationRecord> _backStack = new Stack<NavigationRecord>();
        private readonly Stack<NavigationRecord> _forwardStack = new Stack<NavigationRecord>();

        private IRouteComponent _routeComponent;
        public ITangdaoPage CurrentPage { get; private set; }
        private object _currentView;

        public IRouteComponent RouteComponent
        {
            get => _routeComponent;
            set => _routeComponent = value;
        }

        public bool CanGoBack => _backStack.Count > 0;
        public bool CanGoForward => _forwardStack.Count > 0;

        public object CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<RouteChangedEventArgs> RouteChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RegisterRoute(string route, Func<ITangdaoPage> pageFactory)
        {
            _routeMap[route] = pageFactory;
        }

        public void RegisterPage<T>() where T : ITangdaoPage
        {
            var type = typeof(T);
            var route = GenerateRouteName(type);

            // 注册路由，但实际创建由 RouteComponent 处理
            RegisterRoute(route, () =>
            {
                if (_routeComponent == null)
                    throw new InvalidOperationException("RouteComponent is not set");
                return _routeComponent.ResolvePage(route);
            });

            _reverseRouteMap[type] = route;
        }

        public void NavigateTo<T>(ITangdaoParameter parameters = null) where T : ITangdaoPage
        {
            var type = typeof(T);
            if (!_reverseRouteMap.TryGetValue(type, out var route))
                throw new ArgumentException($"Page type {type.Name} is not registered");

            NavigateTo(route, parameters);
        }

        public void NavigateTo(string route, ITangdaoParameter parameters = null)
        {
            if (_routeMap.TryGetValue(route, out var pageFactory))
            {
                // 使用注册的工厂方法创建页面
                var newPage = pageFactory();
                NavigateToInternal(newPage, parameters);
            }
            else
            {
                throw new ArgumentException($"Route '{route}' is not registered");
            }
        }

        private void NavigateToInternal(ITangdaoPage newPage, ITangdaoParameter parameters = null)
        {
            if (CurrentPage?.CanNavigateAway() == false)
                return;

            var previousPage = CurrentPage;

            if (previousPage != null)
            {
                previousPage.OnNavigatedFrom();
                // 记录到后退栈
                if (_reverseRouteMap.TryGetValue(previousPage.GetType(), out var previousRoute))
                {
                    _backStack.Push(new NavigationRecord(previousRoute, previousPage.GetType(), parameters));
                }
            }

            CurrentPage = newPage;
            CurrentView = ResolveViewForPage(newPage);

            newPage.OnNavigatedTo(parameters);
            _forwardStack.Clear();

            // 通知导航状态变化
            RouteChanged?.Invoke(this, new RouteChangedEventArgs(previousPage, newPage, parameters));
            OnNavigationStateChanged();
        }

        public void GoBack()
        {
            if (_backStack.Count == 0) return;

            var record = _backStack.Pop();
            // 记录当前页面到前进栈
            if (CurrentPage != null && _reverseRouteMap.TryGetValue(CurrentPage.GetType(), out var currentRoute))
            {
                _forwardStack.Push(new NavigationRecord(currentRoute, CurrentPage.GetType(), null));
            }

            // 使用路由导航而不是直接创建实例
            NavigateTo(record.Route, record.Parameters as ITangdaoParameter);
        }

        public void GoForward()
        {
            if (_forwardStack.Count == 0) return;

            var record = _forwardStack.Pop();
            // 记录当前页面到后退栈
            if (CurrentPage != null && _reverseRouteMap.TryGetValue(CurrentPage.GetType(), out var currentRoute))
            {
                _backStack.Push(new NavigationRecord(currentRoute, CurrentPage.GetType(), null));
            }

            // 使用路由导航而不是直接创建实例
            NavigateTo(record.Route, record.Parameters as ITangdaoParameter);
        }

        // 添加导航状态变化通知
        private void OnNavigationStateChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanGoBack)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanGoForward)));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string GenerateRouteName(Type type)
        {
            var route = type.Name;
            if (route.EndsWith("Page"))
                route = route.Substring(0, route.Length - 4);
            if (route.EndsWith("ViewModel"))
                route = route.Substring(0, route.Length - 9);
            if (route.EndsWith("View"))
                route = route.Substring(0, route.Length - 4);
            return route;
        }

        private object ResolveViewForPage(ITangdaoPage page)
        {
            // 根据约定找到对应的 View
            var viewModelType = page.GetType();

            var viewTypeName = viewModelType.FullName
                .Replace("ViewModel", "View")
                .Replace("ViewModels", "Views");

            // 先尝试在同一程序集中查找，如果使用 Type.GetType(viewTypeName)会报错，因为dll不存在你程序的命名空间，
            // 此时为null，你可以设置数据模板

            var viewType = viewModelType.Assembly.GetType(viewTypeName);
            if (viewType != null)
            {
                var view = Activator.CreateInstance(viewType);

                // 设置 DataContext
                if (view is FrameworkElement frameworkElement)
                {
                    frameworkElement.DataContext = page;
                }

                return view;
            }

            return page;
        }

        private class NavigationRecord
        {
            public string Route { get; }
            public Type PageType { get; }
            public ITangdaoParameter Parameters { get; }

            public NavigationRecord(string route, Type pageType, ITangdaoParameter parameters)
            {
                Route = route;
                PageType = pageType;
                Parameters = parameters;
            }
        }
    }
}