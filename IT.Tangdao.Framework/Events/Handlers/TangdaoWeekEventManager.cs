using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.Events.Handlers
{
    /// <summary>
    /// 自定义弱事件管理器，用于管理TangdaoWeakEvent的消息接收事件
    /// </summary>
    internal class TangdaoWeekEventManager : WeakEventManager
    {
        #region 单例模式

        private static TangdaoWeekEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(TangdaoWeekEventManager);
                var manager = (TangdaoWeekEventManager)GetCurrentManager(managerType);

                if (manager == null)
                {
                    manager = new TangdaoWeekEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        #endregion 单例模式

        #region 公共方法

        /// <summary>
        /// 添加消息接收事件处理程序
        /// </summary>
        /// <param name="source">事件源</param>
        /// <param name="handler">事件处理程序</param>
        public static void AddHandler(TangdaoWeakEvent source, EventHandler<MessageEventArgs> handler)
        {
            if (source == null || handler == null)
                return;

            CurrentManager.ProtectedAddHandler(source, handler);
        }

        /// <summary>
        /// 移除消息接收事件处理程序
        /// </summary>
        /// <param name="source">事件源</param>
        /// <param name="handler">事件处理程序</param>
        public static void RemoveHandler(TangdaoWeakEvent source, EventHandler<MessageEventArgs> handler)
        {
            if (source == null || handler == null)
                return;

            CurrentManager.ProtectedRemoveHandler(source, handler);
        }

        #endregion 公共方法

        #region 事件处理

        /// <summary>
        /// 开始监听事件源
        /// </summary>
        /// <param name="source">事件源</param>
        protected override void StartListening(object source)
        {
            var weakEventSource = source as TangdaoWeakEvent;
            if (weakEventSource == null)
                return;

            // 获取内部事件并订阅
            var messageReceivedEvent = typeof(TangdaoWeakEvent).GetEvent("MessageReceived",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (messageReceivedEvent != null)
            {
                // 创建事件处理程序
                var handler = new EventHandler<MessageEventArgs>(OnMessageReceived);
                // 订阅内部事件
                messageReceivedEvent.AddEventHandler(weakEventSource, handler);
            }
        }

        /// <summary>
        /// 停止监听事件源
        /// </summary>
        /// <param name="source">事件源</param>
        protected override void StopListening(object source)
        {
            var weakEventSource = source as TangdaoWeakEvent;
            if (weakEventSource == null)
                return;

            // 获取内部事件并取消订阅
            var messageReceivedEvent = typeof(TangdaoWeakEvent).GetEvent("MessageReceived",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (messageReceivedEvent != null)
            {
                // 创建事件处理程序
                var handler = new EventHandler<MessageEventArgs>(OnMessageReceived);
                // 取消订阅内部事件
                messageReceivedEvent.RemoveEventHandler(weakEventSource, handler);
            }
        }

        /// <summary>
        /// 处理消息接收事件
        /// </summary>
        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            // 使用DeliverEvent方法传递事件给所有订阅者
            DeliverEvent(sender, e);
        }

        #endregion 事件处理
    }

    /// <summary>
    /// 自定义事件管理器
    /// </summary>
    public class TangdaoKeyWeekEventManager
    {
        private KeyMessageEventArgs _keyMessageEventArgs;

        public KeyMessageEventArgs KeyMessageEventArgs
        {
            get => _keyMessageEventArgs;
            set
            {
                KeyMessageReceived.Invoke(this, value);
            }
        }

        public event EventHandler<KeyMessageEventArgs> KeyMessageReceived;
    }

    /// <summary>
    /// 自定义弱事件管理器，用于管理TangdaoWeakEvent的处理器表接收事件
    /// </summary>
    public class TangdaoHandlerTableEventManager
    {
        public static event EventHandler<MessageEventArgs> MessageReceived;

        public static void Publish(MessageEventArgs eventArgs)
        {
            MessageReceived.Invoke(null, eventArgs);
        }
    }
}