using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Results
{
    /// <summary>
    /// 通用查询结果基类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class QueryableResult<T> : QueryableResult, IQueryableResult<T>
    {
        public T Data { get; set; }

        protected void Initialize(bool isSuccess, string message, T data = default,
                               Exception exception = null, string operationType = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            Exception = exception;
            OperationType = operationType ?? typeof(T).Name;
        }

        // 快速创建方法
        public static QueryableResult<T> Success(T data, string message = "操作成功", string operationType = null)
        {
            return new QueryableResult<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                OperationType = operationType
            };
        }

        public new static QueryableResult<T> Failure(string message, Exception exception = null, string operationType = null)
        {
            return new QueryableResult<T>
            {
                IsSuccess = false,
                Message = message,
                Exception = exception,
                OperationType = operationType
            };
        }

        public new static QueryableResult<T> FromException(Exception ex, string operationType = null)
        {
            return Failure($"操作异常: {ex.Message}", ex, operationType);
        }

        // 添加扩展属性
        public new QueryableResult<T> WithProperty(string key, object value)
        {
            ExtendedProperties[key] = value;
            return this;
        }

        public T GetCuttentData()
        {
            return Data;
        }

        // 隐式转换
        public static implicit operator QueryableResult<T>(T data) => Success(data);

        public static implicit operator bool(QueryableResult<T> result) => result.IsSuccess;
    }
}