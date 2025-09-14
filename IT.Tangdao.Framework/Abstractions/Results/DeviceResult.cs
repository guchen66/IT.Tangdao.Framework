using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Results
{
    /// <summary>
    /// 设备操作结果（专为硬件设备设计）
    /// </summary>
    public class DeviceResult : QueryableResult
    {
        /// <summary>
        /// 设备状态码
        /// </summary>
        public DeviceStatus StatusCode { get; set; }

        /// <summary>
        /// 命令执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }

        public string RawResponse { get; set; }

        public int ResponseCode { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        public static DeviceResult Success(string deviceId = null, string message = "设备操作成功",
                                        DeviceStatus statusCode = DeviceStatus.Success,
                                        string rawResponse = null, int responseCode = 200,
                                        long executionTimeMs = 0)
        {
            return new DeviceResult
            {
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode,
                DeviceId = deviceId,
                RawResponse = rawResponse,
                ResponseCode = responseCode,
                ExecutionTimeMs = executionTimeMs
            };
        }

        public static DeviceResult Failure(string message, DeviceStatus statusCode = DeviceStatus.Error,
                                        Exception exception = null, string deviceId = null,
                                        string rawResponse = null, int responseCode = 500,
                                        long executionTimeMs = 0)
        {
            return new DeviceResult
            {
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode,
                Exception = exception,
                DeviceId = deviceId,
                RawResponse = rawResponse,
                ResponseCode = responseCode,
                ExecutionTimeMs = executionTimeMs
            };
        }

        public static DeviceResult FromHardwareResponse(string rawResponse, int responseCode,
                                                     string deviceId, long executionTimeMs)
        {
            var isSuccess = responseCode >= 200 && responseCode < 300;
            var statusCode = isSuccess ? DeviceStatus.Success : DeviceStatus.CommunicationError;
            var message = isSuccess ? "设备响应成功" : "设备响应错误";

            return new DeviceResult
            {
                IsSuccess = isSuccess,
                Message = message,
                StatusCode = statusCode,
                DeviceId = deviceId,
                RawResponse = rawResponse,
                ResponseCode = responseCode,
                ExecutionTimeMs = executionTimeMs
            };
        }
    }
}