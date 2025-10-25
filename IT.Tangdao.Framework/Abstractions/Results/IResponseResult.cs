using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Results
{
    /// <summary>
    /// 响应结果接口
    /// </summary>
    public interface IResponseResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// 结果消息
        /// </summary>
        string Message { get; }

        /// <summary>
        /// 操作时间戳
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// 异常信息（如果有）
        /// </summary>
        Exception Exception { get; }
    }
}