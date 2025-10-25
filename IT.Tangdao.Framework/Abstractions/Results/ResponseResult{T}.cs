using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Results
{
    /// <summary>
    /// 响应结果泛型基类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ResponseResult<T> : ResponseResult, IResponseResult<T>
    {
        public new T Data { get; set; }

        protected void Initialize(bool isSuccess, string message, T data = default, Exception exception = null, string value = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            Exception = exception;
            Value = value ?? typeof(T).Name;
        }

        // 快速创建方法
        public static ResponseResult<T> Success(T data, string message = "操作成功", string value = null)
        {
            return new ResponseResult<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                Value = value
            };
        }

        public new static ResponseResult<T> Failure(string message, Exception exception = null, string value = null)
        {
            return new ResponseResult<T>
            {
                IsSuccess = false,
                Message = message,
                Exception = exception,
                Value = value
            };
        }

        public new static ResponseResult<T> FromException(Exception ex, string operationType = null)
        {
            return Failure($"操作异常: {ex.Message}", ex, operationType);
        }

        // 添加扩展属性
        public new ResponseResult<T> WithProperty(string key, object value)
        {
            ExtendedProperties[key] = value;
            return this;
        }

        public T GetCuttentData()
        {
            return Data;
        }

        // 隐式转换
        public static implicit operator ResponseResult<T>(T data) => Success(data);

        public static implicit operator bool(ResponseResult<T> result) => result.IsSuccess;
    }
}