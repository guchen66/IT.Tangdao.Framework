using IT.Tangdao.Framework.EventArg;
using System.Windows;
using System;
using IT.Tangdao.Framework.DaoTasks;
using IT.Tangdao.Framework.Enums;
using System.Collections.Generic;
using System.Windows.Input;
using IT.Tangdao.Framework.Commands;
using System.ComponentModel;

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
            add => WeakEventManager<TangdaoWeakEvent, MessageEventArgs>.AddHandler(this, nameof(MessageReceived), value);
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

        public void Publish(string key, IHandlerTable handlerTable)
        {
            var args = new HandlerTableEventArgs(key, handlerTable);
            TangdaoTaskScheduler.Execute(dao =>
            {
                HandlerTableReceived?.Invoke(this, args);
            }, TaskThreadType.UI);
        }
    }

    /// <summary>
    /// 自定义弱引用事件管理器，不依赖反射
    /// </summary>
    /// <typeparam name="TSource">事件源类型</typeparam>
    /// <typeparam name="TEventArgs">事件参数类型</typeparam>
    public class CustomWeakEventManager<TSource, TEventArgs> where TSource : class where TEventArgs : EventArgs
    {
        // 存储弱引用订阅者
        private readonly List<WeakReference<EventHandler<TEventArgs>>> _subscribers =
            new List<WeakReference<EventHandler<TEventArgs>>>();

        /// <summary>
        /// 添加弱引用订阅
        /// </summary>
        public void AddHandler(EventHandler<TEventArgs> handler)
        {
            if (handler != null)
            {
                _subscribers.Add(new WeakReference<EventHandler<TEventArgs>>(handler));
            }
        }

        /// <summary>
        /// 移除弱引用订阅
        /// </summary>
        public void RemoveHandler(EventHandler<TEventArgs> handler)
        {
            if (handler != null)
            {
                _subscribers.RemoveAll(wr =>
                {
                    if (wr.TryGetTarget(out var target))
                    {
                        return target == handler;
                    }
                    return true; // 清理已回收的弱引用
                });
            }
        }

        /// <summary>
        /// 触发事件，自动清理已回收的弱引用
        /// </summary>
        public void RaiseEvent(object sender, TEventArgs e)
        {
            // 遍历并触发事件，同时清理无效引用
            for (int i = _subscribers.Count - 1; i >= 0; i--)
            {
                var wr = _subscribers[i];
                if (wr.TryGetTarget(out var handler))
                {
                    handler(sender, e);
                }
                else
                {
                    // 清理已回收的弱引用
                    _subscribers.RemoveAt(i);
                }
            }
        }
    }

    public sealed class TangdaoWeakEvent2
    {
        private static readonly TangdaoWeakEvent2 _instance = new TangdaoWeakEvent2();
        public static TangdaoWeakEvent2 Instance => _instance;

        // 1. 将事件改为 private，完全隐藏内部实现
        private event EventHandler<MessageEventArgs> MessageReceived;

        private event EventHandler<KeyMessageEventArgs> KeyMessageReceived;

        private event EventHandler<HandlerTableEventArgs> HandlerTableReceived;

        // 2. 使用自定义 WeakEventManager 管理弱引用订阅
        private readonly CustomWeakEventManager<TangdaoWeakEvent, MessageEventArgs> _messageEventManager =
            new CustomWeakEventManager<TangdaoWeakEvent, MessageEventArgs>();

        private readonly CustomWeakEventManager<TangdaoWeakEvent, KeyMessageEventArgs> _keyMessageEventManager =
            new CustomWeakEventManager<TangdaoWeakEvent, KeyMessageEventArgs>();

        private readonly CustomWeakEventManager<TangdaoWeakEvent, HandlerTableEventArgs> _handlerTableEventManager =
            new CustomWeakEventManager<TangdaoWeakEvent, HandlerTableEventArgs>();

        // 3. 公共弱引用订阅入口（直接使用自定义管理器，不依赖反射）
        public event EventHandler<MessageEventArgs> OnMessageReceived
        {
            add => _messageEventManager.AddHandler(value);
            remove => _messageEventManager.RemoveHandler(value);
        }

        public event EventHandler<KeyMessageEventArgs> OnKeyMessageReceived
        {
            add => _keyMessageEventManager.AddHandler(value);
            remove => _keyMessageEventManager.RemoveHandler(value);
        }

        public event EventHandler<HandlerTableEventArgs> OnHandlerTableReceived
        {
            add => _handlerTableEventManager.AddHandler(value);
            remove => _handlerTableEventManager.RemoveHandler(value);
        }

        // 4. 发布事件（同时触发原始事件和弱引用事件）
        public void Publish(MessageEventArgs message)
        {
            TangdaoTaskScheduler.Execute(dao =>
            {
                // 触发原始事件（用于内部订阅）
                MessageReceived?.Invoke(this, message);
                // 触发弱引用事件（用于外部订阅）
                _messageEventManager.RaiseEvent(this, message);
            }, TaskThreadType.UI);
        }

        public void Publish(string key, MessageEventArgs message)
        {
            var kargs = new KeyMessageEventArgs(key, message);
            TangdaoTaskScheduler.Execute(dao =>
            {
                KeyMessageReceived?.Invoke(this, kargs);
                _keyMessageEventManager.RaiseEvent(this, kargs);
            }, TaskThreadType.UI);
        }

        public void Publish(string key, IHandlerTable handlerTable)
        {
            var args = new HandlerTableEventArgs(key, handlerTable);
            TangdaoTaskScheduler.Execute(dao =>
            {
                HandlerTableReceived?.Invoke(this, args);
                _handlerTableEventManager.RaiseEvent(this, args);
            }, TaskThreadType.UI);
        }
    }
}