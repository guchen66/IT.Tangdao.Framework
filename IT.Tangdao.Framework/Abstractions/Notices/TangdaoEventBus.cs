using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public static class TangdaoEventBus
    {
        public static void Publish<TArgs>(TArgs args) where TArgs : TangdaoEventArgs
            => TangdaoPublisher<TArgs>.Instance.Publish(args);

        public static IDisposable Subscribe<TArgs>(IObserver<TArgs> observer)
            => TangdaoPublisher<TArgs>.Instance.Subscribe(observer);
    }
}