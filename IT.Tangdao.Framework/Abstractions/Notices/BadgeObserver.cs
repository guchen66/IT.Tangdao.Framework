using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public sealed class BadgeObserver : NoticeObserverBase
    {
        public override NoticeContext Context => NoticeContexts.Badge;
    }
}