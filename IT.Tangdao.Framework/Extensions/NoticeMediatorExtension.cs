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
        public static FluentNoticeBuilder UseNoticeKit(this NoticeMediator mediator)
            => new FluentNoticeBuilder(mediator, new DefaultNoticeFactory());
    }
}