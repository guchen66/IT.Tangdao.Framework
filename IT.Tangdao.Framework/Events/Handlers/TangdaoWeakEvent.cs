using IT.Tangdao.Framework.EventArg;
using System.Windows;
using System;
using IT.Tangdao.Framework.DaoTasks;
using IT.Tangdao.Framework.Enums;
using System.Collections.Generic;
using System.Windows.Input;
using IT.Tangdao.Framework.Commands;
using System.ComponentModel;
using IT.Tangdao.Framework.Events.Handlers;

namespace IT.Tangdao.Framework.Events
{
    public sealed class TangdaoWeakEvent
    {
        private static readonly TangdaoWeakEvent _instance = new TangdaoWeakEvent();
        public static TangdaoWeakEvent Instance => _instance;

        [Obsolete("仅供框架内部使用，请订阅 OnXxxReceived", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<MessageEventArgs> MessageReceived;

        [Obsolete("仅供框架内部使用，请订阅 OnXxxReceived", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<KeyMessageEventArgs> KeyMessageReceived;

        [Obsolete("仅供框架内部使用，请订阅 OnXxxReceived", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<HandlerTableEventArgs> HandlerTableReceived;

        public event EventHandler<MessageEventArgs> OnMessageReceived
        {
            add => WeakEventManager<TangdaoWeakEvent, MessageEventArgs>.AddHandler(this, "MessageReceived", value);
            remove => WeakEventManager<TangdaoWeakEvent, MessageEventArgs>.RemoveHandler(this, nameof(MessageReceived), value);
        }

        public event EventHandler<KeyMessageEventArgs> OnKeyMessageReceived
        {
            add => WeakEventManager<TangdaoWeakEvent, KeyMessageEventArgs>.AddHandler(this, nameof(KeyMessageReceived), value);
            remove => WeakEventManager<TangdaoWeakEvent, KeyMessageEventArgs>.RemoveHandler(this, nameof(KeyMessageReceived), value);
        }

        public event EventHandler<HandlerTableEventArgs> OnHandlerTableReceived
        {
            add => WeakEventManager<TangdaoWeakEvent, HandlerTableEventArgs>.AddHandler(this, nameof(HandlerTableReceived), value);
            remove => WeakEventManager<TangdaoWeakEvent, HandlerTableEventArgs>.RemoveHandler(this, nameof(HandlerTableReceived), value);
        }

        public void Publish(MessageEventArgs message)
        {
            TangdaoTaskScheduler.Execute(dao =>
            {
                MessageReceived?.Invoke(this, message);
            }, TaskThreadType.UI);
        }

        public void Publish(string key, MessageEventArgs message)
        {
            var kargs = new KeyMessageEventArgs(key, message);
            TangdaoTaskScheduler.Execute(dao =>
            {
                KeyMessageReceived?.Invoke(this, kargs);
            }, TaskThreadType.UI);
        }

        public void Publish(string key, IActionTable handlerTable)
        {
            var args = new HandlerTableEventArgs(key, handlerTable);
            TangdaoTaskScheduler.Execute(dao =>
            {
                HandlerTableReceived?.Invoke(this, args);
            }, TaskThreadType.UI);
        }
    }
}