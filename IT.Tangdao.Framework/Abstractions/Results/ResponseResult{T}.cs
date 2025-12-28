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
        public T Data { get; set; }

        T IResponseResult<T>.Data => Data;  // 显式实现，接口调用只能读

        // 索引器支持
        public dynamic this[int index]
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException($"操作失败，无法访问索引: {Message}");
                }

                if (Data == null)
                {
                    throw new NullReferenceException("数据为空，无法访问索引");
                }

                // 处理List类型
                if (Data is System.Collections.IList list)
                {
                    return list[index];
                }

                // 处理数组类型
                var dataType = Data.GetType();
                if (dataType.IsArray)
                {
                    Array array = Data as Array;
                    if (index >= 0 && index < array.Length)
                    {
                        return array.GetValue(index);
                    }
                    throw new IndexOutOfRangeException($"索引 {index} 超出数组范围，数组长度: {array.Length}");
                }

                throw new InvalidOperationException($"类型 {Data.GetType().Name} 不支持索引访问");
            }
        }

        protected internal void Initialize(bool isSuccess, string message, T data = default, Exception exception = null, string value = null)
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
            var result = new ResponseResult<T>();
            result.Initialize(true, message, data, null, value);
            return result;
        }

        public new static ResponseResult<T> Failure(string message, Exception exception = null, string value = null)
        {
            var result = new ResponseResult<T>();
            result.Initialize(false, message, default, exception, value);
            return result;
        }

        public new static ResponseResult<T> FromException(Exception ex, string operationType = null)
        {
            return Failure($"操作异常: {ex.Message}", ex, operationType);
        }

        // 隐式转换
        public static implicit operator ResponseResult<T>(T data) => Success(data);

        public static implicit operator bool(ResponseResult<T> result) => result.IsSuccess;
    }
}