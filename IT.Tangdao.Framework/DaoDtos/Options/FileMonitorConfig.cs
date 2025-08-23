using IT.Tangdao.Framework.DaoEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDtos.Options
{
    /// <summary>
    /// 文件监控配置类
    /// </summary>
    public class FileMonitorConfig
    {
        /// <summary>
        /// 要监控的根目录路径
        /// </summary>
        public string MonitorRootPath { get; set; }

        /// <summary>
        /// 是否搜索子目录
        /// </summary>
        public bool IncludeSubdirectories { get; set; } = true;

        /// <summary>
        /// 要监控的文件类型列表
        /// </summary>
        public List<DaoFileType> MonitorFileTypes { get; set; } = new List<DaoFileType>();

        /// <summary>
        /// 文件变化防抖时间（毫秒）
        /// </summary>
        public int DebounceMilliseconds { get; set; } = 800;

        /// <summary>
        /// 文件读取重试次数
        /// </summary>
        public int FileReadRetryCount { get; set; } = 3;

        /// <summary>
        /// 文件读取重试间隔（毫秒）
        /// </summary>
        public int FileReadRetryDelay { get; set; } = 50;

        /// <summary>
        /// 是否忽略临时文件
        /// </summary>
        public bool IgnoreTemporaryFiles { get; set; } = true;

        /// <summary>
        /// 临时文件模式（用于过滤）
        /// </summary>
        public List<string> TemporaryFilePatterns { get; set; } = new List<string>
        {
            "*.tmp", "~$*", "*.temp", "*.bak"
        };
    }
}