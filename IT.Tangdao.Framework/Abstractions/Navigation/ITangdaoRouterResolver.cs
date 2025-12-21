using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Notices;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Abstractions.Navigation
{
    /// <summary>
    /// 路由解析器接口，用于解析路由并创建页面实例
    /// 替代原有的IRouteComponent，提供更灵活的页面解析功能
    /// </summary>
    public interface ITangdaoRouterResolver
    {
        // ITangdaoPage ResolvePage(RegistrationTypeEntry  noticeRegistry);

        /// <summary>
        /// 根据路由名称解析并创建页面实例
        /// </summary>
        /// <param name="route">路由名称</param>
        /// <returns>创建的页面实例，如果解析失败则返回null</returns>
        ITangdaoPage ResolvePage(RegistrationTypeEntry  route);

        /// <summary>
        /// 根据页面类型解析并创建页面实例
        /// </summary>
        /// <typeparam name="TPage">页面类型，必须实现ITangdaoPage接口</typeparam>
        /// <returns>创建的页面实例，如果解析失败则返回null</returns>
        ITangdaoPage ResolvePage<TPage>() where TPage : class, ITangdaoPage;
    }
}