using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Results
{
    /// <summary>
    /// 非泛型写入结果
    /// </summary>
    public class WriteResult : QueryableResult
    {
        public string Result { get; protected set; }
        public long Size { get; protected set; }
        public string Format { get; protected set; }

        protected WriteResult()
        { }

        // 成功方法
        public static WriteResult Success(string result = null, string message = "写入成功",
                                      long size = 0, string format = null)
        {
            return new WriteResult
            {
                IsSuccess = true,
                Message = message,
                Result = result,
                Size = size,
                Format = format,
                OperationType = "WriteOperation"
            };
        }

        public new static WriteResult Failure(string message, Exception exception = null,
                                     string result = null)
        {
            return new WriteResult
            {
                IsSuccess = false,
                Message = message,
                Exception = exception,
                Result = result,
                OperationType = "WriteOperation"
            };
        }

        public new static WriteResult FromException(Exception exception, string source = null)
        {
            return Failure($"读取异常: {exception.Message}", exception, source);
        }

        /// <summary>
        /// 将非泛型WriteResult转换为泛型WriteResult<T>
        /// </summary>
        public WriteResult<T> ToWriteResult<T>()
        {
            // 如果已经是目标类型，直接返回
            if (this is WriteResult<T> typedResult)
            {
                return typedResult;
            }
            throw new InvalidOperationException($"无法将 {GetType().Name} 转换为 WriteResult<{typeof(T).Name}>");
        }
    }
}