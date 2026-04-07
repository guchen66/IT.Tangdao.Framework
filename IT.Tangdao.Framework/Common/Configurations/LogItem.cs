using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IT.Tangdao.Framework.Configurations
{
    /// <summary>
    /// 日志项
    /// </summary>
    [Serializable]
    public class LogItem
    {
        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadId { get; set; } = Environment.CurrentManagedThreadId;

        /// <summary>
        /// 调用方法名
        /// </summary>
        public string Caller { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LoggerLevel Level { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [XmlIgnore]
        public Exception Exception { get; set; }

        /// <summary>
        /// 异常消息（用于XML序列化）
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// 异常堆栈（用于XML序列化）
        /// </summary>
        public string ExceptionStackTrace { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [XmlIgnore]
        public Type Type { get; set; }

        /// <summary>
        /// 类型名称（用于XML序列化）
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 日志输出方式
        /// </summary>
        public LogOutputType OutputType { get; set; }
    }
}