using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Navigates
{
    /// <summary>
    /// 只有简单的翻页功能，使用拦截器和传输数据请使用ITangdaoRouter
    /// </summary>
    public interface ISingleRouter : INotifyPropertyChanged
    {
        ISingleNavigateView CurrentView { get; }
        bool CanPrevious { get; }
        bool CanNext { get; }
        bool IsAutoRotating { get; set; }

        event EventHandler NavigationChanged;

        void Previous();

        void Next();

        void ToggleAutoCarousel();

        void NavigateTo(ISingleNavigateView view);

        void NavigateToIndex(int index);
    }
}