using IT.Tangdao.Framework.Events;
using System;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LoggerLevel
    {
        /// <summary>
        /// 调试级别，最详细的日志
        /// </summary>
        Debug = 0,

        /// <summary>
        /// 信息级别，一般信息日志
        /// </summary>
        Info = 1,

        /// <summary>
        /// 警告级别，警告信息日志
        /// </summary>
        Warning = 2,

        /// <summary>
        /// 错误级别，错误信息日志
        /// </summary>
        Error = 3,

        /// <summary>
        /// 致命级别，致命错误日志
        /// </summary>
        Fatal = 4
    }
}