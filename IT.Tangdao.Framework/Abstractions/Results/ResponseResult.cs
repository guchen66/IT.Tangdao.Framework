using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Results
{
    /// <summary>
    /// 非泛型响应结果
    /// </summary>
    public class ResponseResult : IResponseResult
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public Exception Exception { get; protected set; }
        public string Value { get; protected set; }

        public object Payload { get; set; }

        //扩展属性字典，用于存储任意附加数据
        public Dictionary<string, object> ExtendedProperties { get; } = new Dictionary<string, object>();

        public static ResponseResult Success(string message = "操作成功", string value = null)
        {
            return new ResponseResult
            {
                IsSuccess = true,
                Message = message,
                Value = value
            };
        }

        public static ResponseResult Success(string message = "操作成功", object payload = null)
        {
            return new ResponseResult
            {
                IsSuccess = true,
                Message = message,
                Payload = payload
            };
        }

        public static ResponseResult Failure(string message, Exception exception = null, string value = null)
        {
            return new ResponseResult
            {
                IsSuccess = false,
                Message = message,
                Exception = exception,
                Value = value
            };
        }

        public static ResponseResult FromException(Exception ex, string content = null)
        {
            return Failure($"操作异常: {ex.Message}", ex, content);
        }

        public ResponseResult WithProperty(string key, object value)
        {
            ExtendedProperties[key] = value;
            return this;
        }

        // 转换为泛型版本
        public ResponseResult<T> ToGenericResult<T>(T data = default)
        {
            var result = new ResponseResult<T>
            {
                IsSuccess = this.IsSuccess,
                Message = this.Message,
                Exception = this.Exception,
                Value = this.Value,
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