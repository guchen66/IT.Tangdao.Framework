using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 委托注册表事件类型
    /// </summary>
    public enum ActionTableEventType
    {
        /// <summary>
        /// 命令注册事件
        /// </summary>
        Registered,

        /// <summary>
        /// 命令移除事件
        /// </summary>
        Unregistered,

        /// <summary>
        /// 命令执行前事件
        /// </summary>
        Executing,

        /// <summary>
        /// 命令执行后事件
        /// </summary>
        Executed
    }
}