using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
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
}