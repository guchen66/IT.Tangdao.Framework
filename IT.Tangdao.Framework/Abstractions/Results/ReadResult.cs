using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Results
{
    /// <summary>
    /// 非泛型读取结果
    /// </summary>
    public class ReadResult : QueryableResult
    {
        public string Result { get; protected set; }
        public long Size { get; protected set; }
        public string Format { get; protected set; }

        protected ReadResult()
        { }

        /// <summary>
        /// 成功返回数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="size"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static ReadResult Success(string result = null, string message = "读取成功", long size = 0, string format = null)
        {
            return new ReadResult
            {
                IsSuccess = true,
                Message = message,
                Result = result,
                Size = size,
                Format = format,
                OperationType = "ReadOperation"
            };
        }

        /// <summary>
        /// 失败返回数据
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ReadResult Failure(string message, Exception exception = null, string result = null)
        {
            return new ReadResult
            {
                IsSuccess = false,
                Message = message,
                Exception = exception,
                Result = result,
                OperationType = "ReadOperation"
            };
        }

        public new static ReadResult FromException(Exception exception, string source = null)
        {
            return Failure($"读取异常: {exception.Message}", exception, source);
        }

        /// <summary>
        /// 将非泛型ReadResult转换为泛型ReadResult<T>
        /// </summary>
        public ReadResult<T> ToReadResult<T>()
        {
            // 如果已经是目标类型，直接返回
            if (this is ReadResult<T> typedResult)
            {
                return typedResult;
            }
            throw new InvalidOperationException($"无法将 {GetType().Name} 转换为 ReadResult<{typeof(T).Name}>");
        }
    }
}