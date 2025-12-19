using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public interface ITangdaoPublisher : IObservable<MessageEventArgs>, IDisposable
    {
        void Publish(MessageEventArgs message);

        void CompleteAll();
    }
}