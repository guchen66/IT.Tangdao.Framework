using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
namespace IT.Tangdao.Framework.Exceptions
{
    /// <summary>
    /// 当操作未在 UI 线程上执行时抛出的异常。
    /// </summary>
    public class NotOnUIThreadException : Exception
    {
        /// <summary>
        /// 使用默认消息初始化异常。
        /// </summary>
        public NotOnUIThreadException()
            : base("当前操作必须在 UI 线程上执行，但检测到当前不在 UI 线程。")
        {
        }

        /// <summary>
        /// 使用指定的消息初始化异常。
        /// </summary>
        public NotOnUIThreadException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定的消息和内部异常初始化异常。
        /// </summary>
        public NotOnUIThreadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 检查当前线程是否为 UI 线程，如果不是则抛出此异常。
        /// </summary>
        /// <param name="dispatcher">
        /// 用于判断的 Dispatcher。若不传，则使用 <see cref="Dispatcher.CurrentDispatcher"/>。
        /// </param>
        public static void ThrowIfNotOnUIThread(Dispatcher dispatcher = null)
        {
            if (dispatcher==null)
            {
                dispatcher = Dispatcher.CurrentDispatcher;
            }
           
            if (!dispatcher.CheckAccess())
            {
                throw new NotOnUIThreadException(
                    $"当前操作必须在 UI 线程（Dispatcher 线程 ID: {dispatcher.Thread.ManagedThreadId}）上执行，" +
                    $"但当前线程 ID 为 {Environment.CurrentManagedThreadId}。");
            }
        }

        /// <summary>
        /// 判断当前线程是否为 UI 线程。
        /// </summary>
        /// <param name="dispatcher">
        /// 用于判断的 Dispatcher。若不传，则使用 <see cref="Dispatcher.CurrentDispatcher"/>。
        /// </param>
        /// <returns>如果是 UI 线程返回 true，否则返回 false。</returns>
        public static bool IsOnUIThread(Dispatcher dispatcher = null)
        {
            if (dispatcher == null)
            {
                dispatcher = Dispatcher.CurrentDispatcher;
            }
            return dispatcher.CheckAccess();
        }
    }
}