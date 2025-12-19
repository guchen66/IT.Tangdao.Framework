using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public class TangdaoPublisher<TArgs> : TangdaoPublisher, ITangdaoPublisher<TArgs>
    {
        public void Publish(TArgs message)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<TArgs> observer)
        {
            throw new NotImplementedException();
        }
    }
}