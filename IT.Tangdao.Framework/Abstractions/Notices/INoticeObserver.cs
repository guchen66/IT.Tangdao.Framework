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

    // 1. 徽章未读数
    //public sealed class BadgeObserver1 : NoticeObserverBase
    //{
    //    public override NoticeContext Context => NoticeContexts.Badge;
    //}

    //// 2. 弹窗/提示
    //public sealed class AlertObserver1 : NoticeObserverBase
    //{
    //    public override NoticeContext Context => NoticeContexts.Alert;
    //}

    public static class NoticeContexts
    {
        public static readonly NoticeContext Badge = new NoticeContext("Badge");
        public static readonly NoticeContext Alert = new NoticeContext("Alert");
    }
}