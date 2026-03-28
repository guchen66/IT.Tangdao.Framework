using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Configurations
{
    /// <summary>
    /// 文件监控运行时信息/状态（动态状态）
    /// </summary>
    public class FileMonitorInfo
    {
        /// <summary>
        /// 监控实例唯一标识
        /// </summary>
        public string MonitorId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 是否正在监控中
        /// </summary>
        public bool IsMonitoring { get; set; }

        /// <summary>
        /// 监控启动时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime? LastActivityTime { get; set; }

        /// <summary>
        /// 关联的配置（引用或ID）
        /// </summary>
        public FileMonitorConfig Config { get; set; }

        /// <summary>
        /// 当前监控的文件统计
        /// </summary>
        public int WatchedFileCount { get; set; }
    }
}