using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 监控状态枚举
    /// </summary>
    public enum MonitorStatus
    {
        /// <summary>
        /// 启动中
        /// </summary>
        Starting,

        /// <summary>
        /// 正在监控
        /// </summary>
        Monitoring,

        /// <summary>
        /// 停止中
        /// </summary>
        Stopping,

        /// <summary>
        /// 已停止
        /// </summary>
        Stopped,

        /// <summary>
        /// 报错
        /// </summary>
        Error
    }
}