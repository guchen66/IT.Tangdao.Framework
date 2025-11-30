using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 日志输出方式枚举
    /// </summary>
    [Flags]
    public enum LogOutputType
    {
        /// <summary>
        /// 不输出日志
        /// </summary>
        None = 0,

        /// <summary>
        /// 输出到文件
        /// </summary>
        File = 1,

        /// <summary>
        /// 输出到控制台
        /// </summary>
        Console = 2,

        /// <summary>
        /// 输出到Debug窗口
        /// </summary>
        Debug = 4,

        /// <summary>
        /// 输出到所有方式
        /// </summary>
        All = File | Console | Debug
    }
}