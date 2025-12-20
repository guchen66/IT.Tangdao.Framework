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
        void UpdateNotice(NoticeContext context);
    }
}