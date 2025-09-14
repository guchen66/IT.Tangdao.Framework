using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Navigates
{
    public interface ITangdaoPage
    {
        // 页面标题（可选）
        string PageTitle { get; }

        // 页面加载时执行
        void OnNavigatedTo(ITangdaoParameter parameter = null);

        // 页面离开时执行
        void OnNavigatedFrom();

        // 页面是否允许离开（用于阻止导航）
        bool CanNavigateAway();
    }
}