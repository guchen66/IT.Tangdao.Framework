using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Results
{
    /// <summary>
    /// 基础结果接口
    /// </summary>
    /// <summary>
    /// 基础查询结果接口（适用于所有查询操作）
    /// </summary>
    public interface IQueryableResult
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

    /// <summary>
    /// 泛型查询结果接口
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IQueryableResult<T> : IQueryableResult
    {
        /// <summary>
        /// 查询到的数据
        /// </summary>
        T Data { get; }
    }
}