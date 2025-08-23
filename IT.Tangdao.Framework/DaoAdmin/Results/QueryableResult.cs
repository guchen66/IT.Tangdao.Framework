using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Results
{
    /// <summary>
    /// 非泛型查询结果
    /// </summary>
    public class QueryableResult : IQueryableResult
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public Exception Exception { get; protected set; }
        public string OperationType { get; protected set; }

        //扩展属性字典，用于存储任意附加数据
        public Dictionary<string, object> ExtendedProperties { get; } = new Dictionary<string, object>();

        protected QueryableResult()
        { }

        public static QueryableResult Success(string message = "操作成功", string operationType = null)
        {
            return new QueryableResult
            {
                IsSuccess = true,
                Message = message,
                OperationType = operationType
            };
        }

        public static QueryableResult Failure(string message, Exception exception = null, string operationType = null)
        {
            return new QueryableResult
            {
                IsSuccess = false,
                Message = message,
                Exception = exception,
                OperationType = operationType
            };
        }

        public static QueryableResult FromException(Exception ex, string operationType = null)
        {
            return Failure($"操作异常: {ex.Message}", ex, operationType);
        }

        public QueryableResult WithProperty(string key, object value)
        {
            ExtendedProperties[key] = value;
            return this;
        }

        // 转换为泛型版本
        public QueryableResult<T> ToGenericResult<T>(T data = default)
        {
            var result = new QueryableResult<T>
            {
                IsSuccess = this.IsSuccess,
                Message = this.Message,
                Exception = this.Exception,
                OperationType = this.OperationType,
                Data = data
            };

            foreach (var prop in this.ExtendedProperties)
            {
                result.ExtendedProperties[prop.Key] = prop.Value;
            }

            return result;
        }
    }
}