using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.EventArg
{
    public abstract class TangdaoEventArgs : EventArgs
    {
        public DateTime NowTime { get; set; }
    }
}