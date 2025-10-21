using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.EventArg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Navigates
{
    /// <summary>
    /// 导航
    /// </summary>
    public interface ITangdaoRouter
    {
        /// <summary>
        /// 后退
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// 前进
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// 导航配套组件，需要总界面实现
        /// </summary>
        IRouteComponent RouteComponent { get; set; }

        /// <summary>
        /// 导航到指定页面类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        void NavigateTo<T>(ITangdaoParameter parameters = null) where T : ITangdaoPage;

        /// <summary>
        /// 导航到指定路由
        /// </summary>
        /// <param name="route"></param>
        /// <param name="parameters"></param>
        void NavigateTo(string route, ITangdaoParameter parameters = null);

        /// <summary>
        /// 导航历史操作
        /// </summary>
        void GoBack();

        /// <summary>
        /// 导航历史操作
        /// </summary>
        void GoForward();

        /// <summary>
        /// 当前页面
        /// </summary>
        ITangdaoPage CurrentPage { get; }

        /// <summary>
        /// 路由变化事件
        /// </summary>
        event EventHandler<RouteChangedEventArgs> RouteChanged;

        /// <summary>
        /// 注册路由
        /// </summary>
        /// <param name="route"></param>
        /// <param name="pageFactory"></param>
        void RegisterRoute(string route, Func<ITangdaoPage> pageFactory);

        /// <summary>
        /// 注册路由
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RegisterPage<T>() where T : ITangdaoPage;
    }
}