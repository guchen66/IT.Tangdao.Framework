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

    public sealed class FluentNoticeBuilder
    {
        private readonly NoticeMediator _m;
        private readonly INoticeFactory _f;

        internal FluentNoticeBuilder(NoticeMediator m, INoticeFactory f)
        {
            _m = m;
            _f = f;
        }

        /// <summary>
        /// 一键创建并注册指定 Key 的观察者
        /// </summary>
        public FluentNoticeBuilder Add(string key, object parameter = null)
        {
            var ctx = new NoticeContext(key, parameter);
            var obs = _f.CreateObserver(ctx);
            _m.Register(obs);
            return this;   // 链式
        }

        /* 如果愿意，可以继续链其它配置，例如
        public FluentNoticeBuilder AddSound() => Add("Sound");
        public FluentNoticeBuilder AddBadge() => Add("Badge");
        */
    }
}