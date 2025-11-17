using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
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
    }
}