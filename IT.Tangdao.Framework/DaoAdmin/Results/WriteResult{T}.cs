using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Results
{
    /// <summary>
    /// 泛型写入结果
    /// </summary>
    public class WriteResult<T> : WriteResult, IQueryableResult<T>
    {
        public T Data { get; protected set; }

        // 成功方法
        public static WriteResult<T> Success(T data, string result = null,
                                         string message = "写入成功",
                                         long size = 0, string format = null)
        {
            return new WriteResult<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                Result = result,
                Size = size,
                Format = format,
                OperationType = "WriteOperation"
            };
        }

        // 失败方法（泛型版本需要）
        public new static WriteResult<T> Failure(string message, Exception exception = null,
                                             string result = null)
        {
            return new WriteResult<T>
            {
                IsSuccess = false,
                Message = message,
                Exception = exception,
                Result = result,
                OperationType = "WriteOperation"
            };
        }

        public new static WriteResult<T> FromException(Exception ex, string rawValue = null)
        {
            return Failure($"写入过程中发生异常: {ex.Message}", ex, rawValue);
        }

        public T GetCuttentData()
        {
            return Data;
        }
    }
}