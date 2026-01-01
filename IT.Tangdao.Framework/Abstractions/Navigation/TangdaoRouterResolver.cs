using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Abstractions.Navigation
{
    /// <summary>
    /// 默认路由解析器实现
    /// 提供基于类型名称的页面解析功能
    /// </summary>
    public class TangdaoRouterResolver : ITangdaoRouterResolver
    {
        private readonly Func<IRegistrationTypeEntry, ITangdaoPage> _customResolver;

        /// <summary>
        /// 初始化默认路由解析器实例
        /// </summary>
        public TangdaoRouterResolver() : this(null)
        { }

        /// <summary>
        /// 使用自定义解析函数初始化路由解析器实例
        /// </summary>
        /// <param name="customResolver">自定义解析函数</param>
        public TangdaoRouterResolver(Func<IRegistrationTypeEntry, ITangdaoPage> customResolver = null)
        {
            _customResolver = customResolver;
        }

        /// <summary>
        /// 根据路由名称解析并创建页面实例
        /// </summary>
        /// <param name="route">路由名称</param>
        /// <returns>创建的页面实例，如果解析失败则返回null</returns>
        public ITangdaoPage ResolvePage(IRegistrationTypeEntry route)
        {
            // 如果提供了自定义解析函数，优先使用
            if (_customResolver != null)
            {
                return _customResolver(route);
            }

            // 默认实现：尝试使用内置IOC容器解析
            try
            {
                return TangdaoApplication.Provider.GetKeyedService<ITangdaoPage>(route.Key);
            }
            catch (Exception)
            {
                // 解析失败时返回null
                return null;
            }
        }

        /// <summary>
        /// 根据页面类型解析并创建页面实例
        /// </summary>
        /// <typeparam name="TPage">页面类型，必须实现ITangdaoPage接口</typeparam>
        /// <returns>创建的页面实例，如果解析失败则返回null</returns>
        public ITangdaoPage ResolvePage<TPage>() where TPage : class, ITangdaoPage
        {
            try
            {
                return TangdaoApplication.Provider.GetService<TPage>();
            }
            catch (Exception)
            {
                // 解析失败时返回null
                return null;
            }
        }
    }
}