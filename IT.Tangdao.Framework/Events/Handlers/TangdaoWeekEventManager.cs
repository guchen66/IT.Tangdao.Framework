using System;
using System.Collections.Generic;
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
    /// 自定义弱事件管理器，用于管理TangdaoWeakEvent的带键消息接收事件
    /// </summary>
    internal class TangdaoKeyWeekEventManager : WeakEventManager
    {
        #region 单例模式

        private static TangdaoKeyWeekEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(TangdaoKeyWeekEventManager);
                var manager = (TangdaoKeyWeekEventManager)GetCurrentManager(managerType);

                if (manager == null)
                {
                    manager = new TangdaoKeyWeekEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        #endregion 单例模式

        #region 公共方法

        /// <summary>
        /// 添加带键消息接收事件处理程序
        /// </summary>
        /// <param name="source">事件源</param>
        /// <param name="handler">事件处理程序</param>
        public static void AddHandler(TangdaoWeakEvent source, EventHandler<KeyMessageEventArgs> handler)
        {
            if (source == null || handler == null)
                return;

            CurrentManager.ProtectedAddHandler(source, handler);
        }

        /// <summary>
        /// 移除带键消息接收事件处理程序
        /// </summary>
        /// <param name="source">事件源</param>
        /// <param name="handler">事件处理程序</param>
        public static void RemoveHandler(TangdaoWeakEvent source, EventHandler<KeyMessageEventArgs> handler)
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
        protected override void StartListening(object source)
        {
            var weakEventSource = source as TangdaoWeakEvent;
            if (weakEventSource == null)
                return;

            // 获取内部事件并订阅
            var keyMessageReceivedEvent = typeof(TangdaoWeakEvent).GetEvent("KeyMessageReceived",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (keyMessageReceivedEvent != null)
            {
                // 创建事件处理程序
                var handler = new EventHandler<KeyMessageEventArgs>(OnKeyMessageReceived);
                // 订阅内部事件
                keyMessageReceivedEvent.AddEventHandler(weakEventSource, handler);
            }
        }

        /// <summary>
        /// 停止监听事件源
        /// </summary>
        protected override void StopListening(object source)
        {
            var weakEventSource = source as TangdaoWeakEvent;
            if (weakEventSource == null)
                return;

            // 获取内部事件并取消订阅
            var keyMessageReceivedEvent = typeof(TangdaoWeakEvent).GetEvent("KeyMessageReceived",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (keyMessageReceivedEvent != null)
            {
                // 创建事件处理程序
                var handler = new EventHandler<KeyMessageEventArgs>(OnKeyMessageReceived);
                // 取消订阅内部事件
                keyMessageReceivedEvent.RemoveEventHandler(weakEventSource, handler);
            }
        }

        /// <summary>
        /// 处理带键消息接收事件
        /// </summary>
        private void OnKeyMessageReceived(object sender, KeyMessageEventArgs e)
        {
            // 使用DeliverEvent方法传递事件给所有订阅者
            DeliverEvent(sender, e);
        }

        #endregion 事件处理
    }

    /// <summary>
    /// 自定义弱事件管理器，用于管理TangdaoWeakEvent的处理器表接收事件
    /// </summary>
    internal class TangdaoHandlerTableWeekEventManager : WeakEventManager
    {
        #region 单例模式

        private static TangdaoHandlerTableWeekEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(TangdaoHandlerTableWeekEventManager);
                var manager = (TangdaoHandlerTableWeekEventManager)GetCurrentManager(managerType);

                if (manager == null)
                {
                    manager = new TangdaoHandlerTableWeekEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        #endregion 单例模式

        #region 公共方法

        /// <summary>
        /// 添加处理器表接收事件处理程序
        /// </summary>
        /// <param name="source">事件源</param>
        /// <param name="handler">事件处理程序</param>
        public static void AddHandler(TangdaoWeakEvent source, EventHandler<HandlerTableEventArgs> handler)
        {
            if (source == null || handler == null)
                return;

            CurrentManager.ProtectedAddHandler(source, handler);
        }

        /// <summary>
        /// 移除处理器表接收事件处理程序
        /// </summary>
        /// <param name="source">事件源</param>
        /// <param name="handler">事件处理程序</param>
        public static void RemoveHandler(TangdaoWeakEvent source, EventHandler<HandlerTableEventArgs> handler)
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
        protected override void StartListening(object source)
        {
            var weakEventSource = source as TangdaoWeakEvent;
            if (weakEventSource == null)
                return;

            // 获取内部事件并订阅
            var handlerTableReceivedEvent = typeof(TangdaoWeakEvent).GetEvent("HandlerTableReceived",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (handlerTableReceivedEvent != null)
            {
                // 创建事件处理程序
                var handler = new EventHandler<HandlerTableEventArgs>(OnHandlerTableReceived);
                // 订阅内部事件
                handlerTableReceivedEvent.AddEventHandler(weakEventSource, handler);
            }
        }

        /// <summary>
        /// 停止监听事件源
        /// </summary>
        protected override void StopListening(object source)
        {
            var weakEventSource = source as TangdaoWeakEvent;
            if (weakEventSource == null)
                return;

            // 获取内部事件并取消订阅
            var handlerTableReceivedEvent = typeof(TangdaoWeakEvent).GetEvent("HandlerTableReceived",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (handlerTableReceivedEvent != null)
            {
                // 创建事件处理程序
                var handler = new EventHandler<HandlerTableEventArgs>(OnHandlerTableReceived);
                // 取消订阅内部事件
                handlerTableReceivedEvent.RemoveEventHandler(weakEventSource, handler);
            }
        }

        /// <summary>
        /// 处理处理器表接收事件
        /// </summary>
        private void OnHandlerTableReceived(object sender, HandlerTableEventArgs e)
        {
            // 使用DeliverEvent方法传递事件给所有订阅者
            DeliverEvent(sender, e);
        }

        #endregion 事件处理
    }
}