using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public interface ITangdaoPublisher<TNotification> : IObservable<TNotification>, IDisposable
    {
        /// <summary>
        /// 发布通知给所有订阅者
        /// </summary>
        /// <param name="notification">要发布的通知</param>
        void Publish(TNotification notification);

        /// <summary>
        /// 完成所有订阅，不再接受新的通知，并清理资源
        /// </summary>
        void CompleteAll();
    }
}