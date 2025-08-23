using IT.Tangdao.Framework.DaoEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Results
{
    /// <summary>
    /// 设备查询结果
    /// </summary>
    public class DeviceResult<T> : DeviceResult, IQueryableResult<T>
    {
        public T Data { get; set; }

        public static DeviceResult<T> Success(T data, string deviceId,
            string message = "设备操作成功", DeviceStatus statusCode = DeviceStatus.Success,
            string rawResponse = null, int responseCode = 200, long executionTimeMs = 0)
        {
            return new DeviceResult<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                DeviceId = deviceId,
                RawResponse = rawResponse,
                ResponseCode = responseCode,
                ExecutionTimeMs = executionTimeMs,
                StatusCode = statusCode,
                OperationType = "DeviceOperation"
            };
        }

        public static DeviceResult<T> Failure(string message, string deviceId = null,
            DeviceStatus statusCode = DeviceStatus.Error, Exception exception = null,
            string rawResponse = null, int responseCode = 500, long executionTimeMs = 0)
        {
            return new DeviceResult<T>
            {
                IsSuccess = false,
                Message = message,
                DeviceId = deviceId,
                StatusCode = statusCode,
                Exception = exception,
                RawResponse = rawResponse,
                ResponseCode = responseCode,
                ExecutionTimeMs = executionTimeMs,
                OperationType = "DeviceOperation"
            };
        }
    }
}