using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public class NoticeState
    {
        public bool IsActive { get; set; }
        public NoticeContext Context { get; set; }
        public DateTime UpdateTime { get; set; }
        public int UnreadCount { get; set; }
    }
}