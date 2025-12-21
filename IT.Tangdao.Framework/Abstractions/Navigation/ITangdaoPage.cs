using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Navigation
{
    /// <summary>
    /// ITangdaoRouter配套页面
    /// </summary>
    public interface ITangdaoPage
    {
        /// <summary>
        /// 页面标题（可选）
        /// </summary>
        string PageTitle { get; }

        /// <summary>
        /// 页面加载时执行
        /// </summary>
        /// <param name="parameter"></param>
        void OnNavigatedTo(ITangdaoParameter parameter = null);

        /// <summary>
        /// 页面离开时执行
        /// </summary>
        void OnNavigatedFrom();

        /// <summary>
        /// 页面是否允许离开（用于阻止导航）
        /// </summary>
        /// <returns></returns>
        bool CanNavigateAway();
    }
}