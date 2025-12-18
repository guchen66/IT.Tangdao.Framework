using IT.Tangdao.Framework.EventArg;
using System.Windows;
using System;
using IT.Tangdao.Framework.DaoTasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Events
{
    public sealed class TangdaoWeakEvent<TArgs> where TArgs : TangdaoEventArgs
    {
        private static readonly TangdaoWeakEvent<TArgs> _instance = new TangdaoWeakEvent<TArgs>();
        public static TangdaoWeakEvent<TArgs> Instance => _instance;

        public event EventHandler<TArgs> MessageReceived;

        public event EventHandler<TArgs> OnMessageReceived
        {
            add => WeakEventManager<TangdaoWeakEvent<TArgs>, TArgs>.AddHandler(this, nameof(MessageReceived), value);
            remove => WeakEventManager<TangdaoWeakEvent<TArgs>, TArgs>.RemoveHandler(this, nameof(MessageReceived), value);
        }

        public void Publish(TArgs args)
        {
            TangdaoTaskScheduler.Execute(_ => MessageReceived?.Invoke(this, args), TaskThreadType.UI);
        }
    }
}