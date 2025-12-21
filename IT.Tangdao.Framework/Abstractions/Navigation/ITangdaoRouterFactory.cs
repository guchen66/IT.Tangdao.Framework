using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Navigation
{
    /// <summary>
    /// 导航器工厂接口
    /// 用于创建和管理ITangdaoRouter实例，实现依赖倒置原则
    /// </summary>
    public interface ITangdaoRouterFactory
    {
        /// <summary>
        /// 创建导航器实例
        /// </summary>
        /// <returns>创建的导航器实例</returns>
        ITangdaoRouter CreateRouter();

        /// <summary>
        /// 设置路由解析器，用于页面解析
        /// </summary>
        /// <param name="resolver">路由解析器实例</param>
        /// <exception cref="ArgumentNullException">当resolver为null时抛出</exception>
        void SetRouteResolver(ITangdaoRouterResolver resolver);

        /// <summary>
        /// 设置自定义路由解析函数
        /// </summary>
        /// <param name="customResolver">自定义路由解析函数</param>
        /// <exception cref="ArgumentNullException">当customResolver为null时抛出</exception>
        void SetRouteResolver(Func<string, ITangdaoPage> customResolver);

        /// <summary>
        /// 获取当前使用的路由解析器
        /// </summary>
        /// <returns>当前使用的路由解析器实例</returns>
        ITangdaoRouterResolver GetCurrentRouteResolver();
    }
}