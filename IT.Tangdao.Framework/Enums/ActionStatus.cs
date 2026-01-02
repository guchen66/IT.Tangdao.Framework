using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 用于描述命令或操作执行后的结果状态，提供了标准化的状态码
    /// </summary>
    public enum ActionStatus
    {
        /// <summary>
        /// 默认状态，表示操作尚未执行或状态未知
        /// </summary>
        None = 0,

        /// <summary>
        /// 表示操作执行成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 表示操作被用户主动取消
        /// </summary>
        Cancel = 2,

        /// <summary>
        /// 表示操作执行过程中发生错误
        /// </summary>
        Error = 3,
    }
}