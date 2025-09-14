using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Results
{
    /// <summary>
    /// 泛型读取结果
    /// </summary>
    public class ReadResult<T> : ReadResult, IQueryableResult<T>
    {
        public T Data { get; protected set; }

        public static ReadResult<T> Success(string result = null)
        {
            return new ReadResult<T>
            {
                IsSuccess = true,
                Result = result,
                OperationType = "ReadOperation"
            };
        }

        // 成功方法
        public static ReadResult<T> Success(T data, string result = null, string message = "读取成功", long size = 0, string format = null)
        {
            return new ReadResult<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                Result = result,
                Size = size,
                Format = format,
                OperationType = "ReadOperation"
            };
        }

        // 失败方法（泛型版本需要）
        public new static ReadResult<T> Failure(string message, Exception exception = null, string result = null)
        {
            return new ReadResult<T>
            {
                IsSuccess = false,
                Message = message,
                Exception = exception,
                Result = result,
                OperationType = "ReadOperation"
            };
        }

        public new static ReadResult<T> FromException(Exception ex, string rawValue = null)
        {
            return Failure($"读取过程中发生异常: {ex.Message}", ex, rawValue);
        }
    }
}