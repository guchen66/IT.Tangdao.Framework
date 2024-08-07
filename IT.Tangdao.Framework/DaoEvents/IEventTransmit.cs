using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEvents
{
    public interface IEventTransmit
    {
        void Publish<T>(object obj = null) where T : DaoEventBase, new();

        void Subscribe<T>(Action<object> executeMethod) where T : DaoEventBase, new();
    }
}
