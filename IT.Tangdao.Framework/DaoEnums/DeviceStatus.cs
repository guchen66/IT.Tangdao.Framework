using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEnums
{
    /// <summary>
    /// 设备状态码
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 一般错误
        /// </summary>
        Error = 1,

        /// <summary>
        /// 通信错误
        /// </summary>
        CommunicationError = 2,

        /// <summary>
        /// 超时
        /// </summary>
        Timeout = 3,

        /// <summary>
        /// 设备忙
        /// </summary>
        Busy = 4,

        /// <summary>
        /// 设备离线
        /// </summary>
        Offline = 5,

        /// <summary>
        /// 参数错误
        /// </summary>
        InvalidParameter = 6,

        /// <summary>
        /// 设备未初始化
        /// </summary>
        NotInitialized = 7,

        /// <summary>
        /// 硬件故障
        /// </summary>
        HardwareFault = 8
    }
}