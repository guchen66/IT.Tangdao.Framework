using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 事件通知器
    /// </summary>
    public class TangdaoNotifier : ITangdaoNotifier
    {
        public void Dispose()
        {
            Console.WriteLine("TangdaoNotifier调用Dispose");
        }

        public void OnCompleted()
        {
            Console.WriteLine("TangdaoNotifier调用OnCompleted");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("TangdaoNotifier调用OnError");
        }

        public void OnNext(MessageEventArgs message)
        {
            Console.WriteLine("TangdaoNotifier调用OnNext");
        }
    }

    public interface ITangdaoNotifier : IObserver<MessageEventArgs>, IDisposable
    {
    }
}