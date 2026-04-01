using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Templates;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Configurations
{
    /// <summary>
    /// 日志配置项
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// 日志保存路径
        /// </summary>
        public string SaveDir { get; set; }

        /// <summary>
        /// 日志间隔
        /// </summary>
        public TangdaoLogInterval LogInterval { get; set; }

        /// <summary>
        /// 日志模板
        /// </summary>
        public TangdaoLogTemplate Template { get; set; }
    }
}