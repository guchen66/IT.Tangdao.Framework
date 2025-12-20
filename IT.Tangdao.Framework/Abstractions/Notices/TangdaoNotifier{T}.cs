using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 泛型通知接收器实现，用于接收特定类型的通知
    /// </summary>
    /// <typeparam name="TNotification">通知类型</typeparam>
    public class TangdaoNotifier<TNotification> : ITangdaoNotifier<TNotification>
    {
        /// <summary>
        /// 通知处理委托
        /// </summary>
        public Action<TNotification> OnNotificationReceived { get; set; }

        /// <summary>
        /// 错误处理委托
        /// </summary>
        public Action<Exception> OnErrorReceived { get; set; }

        /// <summary>
        /// 完成处理委托
        /// </summary>
        public Action OnCompletedReceived { get; set; }

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 接收通知
        /// </summary>
        /// <param name="notification">收到的通知</param>
        public void OnNext(TNotification notification)
        {
            try
            {
                OnNotificationReceived?.Invoke(notification);
            }
            catch (Exception ex)
            {
                // 如果用户提供了错误处理委托，则调用它，否则重新抛出
                if (OnErrorReceived != null)
                {
                    OnErrorReceived(ex);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 处理错误
        /// </summary>
        /// <param name="error">发生的错误</param>
        public void OnError(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error), "错误对象不能为空");
            }

            OnErrorReceived?.Invoke(error);
        }

        /// <summary>
        /// 处理完成通知
        /// </summary>
        public void OnCompleted()
        {
            OnCompletedReceived?.Invoke();
            Dispose();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            // 清理委托引用，避免内存泄漏
            OnNotificationReceived = null;
            OnErrorReceived = null;
            OnCompletedReceived = null;

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}