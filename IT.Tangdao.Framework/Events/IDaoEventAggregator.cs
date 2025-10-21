using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Events
{
    public interface IDaoEventAggregator
    {
        void Publish<T>(T @event = null) where T : DaoEventBase, new();

        Task PublishAsync<T>(T @event = null) where T : DaoEventBase, new();

        void Subscribe<T>(DaoEventHandler<T> handler) where T : DaoEventBase, new();

        void SubscribeAsync<T>(DaoAsyncEventHandler<T> handler) where T : DaoEventBase, new();

        void UnSubscribe<T>(DaoEventHandler<T> handler) where T : DaoEventBase, new();

        void UnSubscribeAsync<T>(DaoAsyncEventHandler<T> handler) where T : DaoEventBase, new();
    }
}