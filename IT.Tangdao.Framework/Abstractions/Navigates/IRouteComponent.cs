using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Navigates
{
    /// <summary>
    /// 注册总界面
    /// </summary>
    public interface IRouteComponent
    {
        /// <summary>
        /// 注册跳转界面
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        ITangdaoPage ResolvePage(string route);
    }
}