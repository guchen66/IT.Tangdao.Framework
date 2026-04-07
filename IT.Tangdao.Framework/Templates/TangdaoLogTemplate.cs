using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Templates
{
    /// <summary>
    /// 日志模板
    /// </summary>
    public class TangdaoLogTemplate
    {
        /// <summary>
        /// 日志格式
        /// </summary>
        public LogContent LoggerFormat { get; set; }

        public enum LogContent
        {
            None = 0,
            Xml,
            Json,
            Txt
        }
    }
}