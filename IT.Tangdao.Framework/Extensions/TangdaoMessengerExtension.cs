using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Messaging;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoMessengerExtension
    {
        /// <summary>
        /// 广播通知链式注册
        /// </summary>
        public static ObserverBuilder ChainRegister(this TangdaoMessenger messenger)
            => new ObserverBuilder(messenger, messenger.ObserverCache);

        /// <summary>
        /// 链式入口：直接注册多个观察者类型
        /// </summary>
        /// <param name="messenger">消息传递者实例</param>
        /// <param name="types">要注册的观察者类型数组，每个类型必须实现IMessageObserver接口</param>
        /// <returns>当前ObserverBuilder实例，用于链式调用</returns>
        public static ObserverBuilder ChainRegister(this TangdaoMessenger messenger, params Type[] types)
        {
            var builder = new ObserverBuilder(messenger, messenger.ObserverCache);
            foreach (var type in types)
            {
                builder.Add(type);
            }
            return builder;
        }
    }
}