using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.EventArg
{
    /// <summary>
    /// 取消事件参数
    /// </summary>
    public class CancellationEventArgs : EventArgs
    {
        /// <summary>
        /// 是否已取消
        /// </summary>
        public bool IsCancelled { get; set; }
    }
}