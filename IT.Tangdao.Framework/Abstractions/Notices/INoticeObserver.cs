using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知观察者接口
    /// </summary>
    public interface INoticeObserver
    {
        void UpdateState(NoticeState state);

        NoticeContext Context { get; }
    }

    public class NoticeState
    {
        public bool IsActive { get; set; }
        public NoticeContext Context { get; set; }
        public DateTime UpdateTime { get; set; }
        public int UnreadCount { get; set; }
    }

    public abstract class NoticeObserverBase : INotifyPropertyChanged, INoticeObserver
    {
        private bool _isActive;
        private int _unreadCount;

        public bool IsActive
        {
            get => _isActive;
            protected set { _isActive = value; OnPropertyChanged(); }
        }

        public int UnreadCount
        {
            get => _unreadCount;
            protected set { _unreadCount = value; OnPropertyChanged(); }
        }

        public abstract NoticeContext Context { get; }

        public void UpdateState(NoticeState state)
        {
            IsActive = state.IsActive;
            UnreadCount = state.UnreadCount;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }

    // 1. 徽章未读数
    public sealed class BadgeObserver : NoticeObserverBase
    {
        public override NoticeContext Context => NoticeContexts.Badge;
    }

    // 2. 弹窗/提示
    public sealed class AlertObserver : NoticeObserverBase
    {
        public override NoticeContext Context => NoticeContexts.Alert;
    }

    public static class NoticeContexts
    {
        public static readonly NoticeContext Badge = new NoticeContext("Badge");
        public static readonly NoticeContext Alert = new NoticeContext("Alert");
    }
}