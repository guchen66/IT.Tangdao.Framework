using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public interface ITangdaoPublisher<TArgs> : IObservable<TArgs>, IDisposable
    {
        void Publish(TArgs message);

        void CompleteAll();
    }
}