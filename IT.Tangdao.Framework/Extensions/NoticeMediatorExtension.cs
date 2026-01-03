using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Notices;

namespace IT.Tangdao.Framework.Extensions
{
    public static class NoticeMediatorExtension
    {
        /// <summary>
        /// 链式入口：持有工厂，后续可继续 .Add().Add()...
        /// </summary>
        public static NoticeBuilder ChainRegister(this NoticeMediator mediator)
            => new NoticeBuilder(mediator, mediator.Resolver);

        /// <summary>
        /// 链式入口：直接注册多个观察者类型
        /// </summary>
        /// <param name="mediator">通知中介者实例</param>
        /// <param name="types">要注册的观察者类型数组，每个类型必须实现INoticeObserver接口</param>
        /// <returns>当前NoticeBuilder实例，用于链式调用</returns>
        public static NoticeBuilder ChainRegister(this NoticeMediator mediator, params Type[] types)
        {
            var builder = new NoticeBuilder(mediator, mediator.Resolver);
            foreach (var type in types)
            {
                builder.Add(type);
            }
            return builder;
        }
    }
}