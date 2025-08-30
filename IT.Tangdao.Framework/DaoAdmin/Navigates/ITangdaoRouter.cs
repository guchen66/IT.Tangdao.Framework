using IT.Tangdao.Framework.DaoEvents;
using IT.Tangdao.Framework.DaoIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Navigates
{
    public interface ITangdaoRouter
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        IRouteComponent RouteComponent { get; set; }

        // 导航到指定页面类型
        void NavigateTo<T>(ITangdaoParameter parameters = null) where T : ITangdaoPage;

        // 导航到指定路由
        void NavigateTo(string route, ITangdaoParameter parameters = null);

        // 导航历史操作
        void GoBack();

        void GoForward();

        // 当前页面
        ITangdaoPage CurrentPage { get; }

        // 路由变化事件
        event EventHandler<RouteChangedEventArgs> RouteChanged;

        // 注册路由
        void RegisterRoute(string route, Func<ITangdaoPage> pageFactory);

        void RegisterPage<T>() where T : ITangdaoPage;
    }
}