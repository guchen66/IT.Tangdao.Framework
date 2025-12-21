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
    /// 导航器工厂实现类
    /// 用于创建和管理ITangdaoRouter实例
    /// </summary>
    public sealed class TangdaoRouterFactory : ITangdaoRouterFactory
    {
        private ITangdaoRouterResolver _routeResolver;

        /// <summary>
        /// 使用指定的路由解析器初始化导航器工厂实例
        /// </summary>
        /// <param name="routeResolver">路由解析器实例，不能为空</param>
        /// <exception cref="ArgumentNullException">当routeResolver为null时抛出</exception>
        public TangdaoRouterFactory(ITangdaoRouterResolver routeResolver)
        {
            _routeResolver = routeResolver ?? throw new ArgumentNullException(nameof(routeResolver));
        }

        /// <summary>
        /// 创建导航器实例
        /// </summary>
        /// <returns>创建的导航器实例</returns>
        public ITangdaoRouter CreateRouter()
        {
            return new TangdaoRouter(_routeResolver);
        }

        /// <summary>
        /// 设置路由解析器，用于页面解析
        /// </summary>
        /// <param name="resolver">路由解析器实例</param>
        /// <exception cref="ArgumentNullException">当resolver为null时抛出</exception>
        public void SetRouteResolver(ITangdaoRouterResolver resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            _routeResolver = resolver;
        }

        /// <summary>
        /// 设置自定义路由解析函数
        /// </summary>
        /// <param name="customResolver">自定义路由解析函数</param>
        /// <exception cref="ArgumentNullException">当customResolver为null时抛出</exception>
        public void SetRouteResolver(Func<string, ITangdaoPage> customResolver)
        {
            if (customResolver == null)
            {
                throw new ArgumentNullException(nameof(customResolver));
            }

            // 适配路由名称解析函数到RegistrationTypeEntry解析函数
            _routeResolver = new TangdaoRouterResolver(reg => customResolver(reg.Key));
        }

        /// <summary>
        /// 获取当前使用的路由解析器
        /// </summary>
        /// <returns>当前使用的路由解析器实例</returns>
        public ITangdaoRouterResolver GetCurrentRouteResolver()
        {
            return _routeResolver;
        }
    }
}