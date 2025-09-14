using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    public enum NetConnectionType
    {
        /// <summary>
        /// 未知模式
        /// </summary>
        Note,

        /// <summary>
        /// Tcp通讯
        /// </summary>
        Tcp,

        /// <summary>
        /// Udp通讯
        /// </summary>
        Udp,

        /// <summary>
        /// 串口通讯
        /// </summary>
        Serial
    }
}