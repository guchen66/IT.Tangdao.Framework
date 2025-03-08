using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public interface IHardwaredevice
    {
        /// <summary>
        /// 从站Id
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        string DeviceName { get; set; }

        /// <summary>
        /// 设备是否连接
        /// </summary>
        bool IsConn { get; set; }

        /// <summary>
        /// 硬件打开
        /// </summary>
        /// <returns></returns>
        Task<IReadResult> Open();

        /// <summary>
        /// 硬件关闭
        /// </summary>
        /// <returns></returns>
        Task<IReadResult> Close();

        Task<IReadResult> Read();
    }
}