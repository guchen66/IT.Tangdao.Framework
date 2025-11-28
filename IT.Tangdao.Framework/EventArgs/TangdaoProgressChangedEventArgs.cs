using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.EventArg
{
    /// <summary>
    /// 进度变化事件参数
    /// </summary>
    public class TangdaoProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 进度百分比
        /// </summary>
        public double ProgressPercentage { get; set; }

        /// <summary>
        /// 状态文本
        /// </summary>
        public string StatusText { get; set; }

        /// <summary>
        /// 总工作量
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 已完成工作量
        /// </summary>
        public long Completed { get; set; }

        /// <summary>
        /// 是否已取消
        /// </summary>
        public bool IsCancelled { get; set; }
    }
}