using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.Commands
{
    /// <summary>
    /// 提供委托表事件通知功能
    /// </summary>
    public interface IActionTableHandler
    {
        /// <summary>
        /// 注册委托时发生的事件
        /// </summary>
        event EventHandler<ActionTableEventArgs> Registered;

        /// <summary>
        /// 移除委托时发生的事件
        /// </summary>
        event EventHandler<ActionTableEventArgs> Unregistered;

        /// <summary>
        /// 委托执行时发生的事件
        /// </summary>
        event EventHandler<ActionTableEventArgs> Executing;

        /// <summary>
        /// 微弱执行后发生的事件
        /// </summary>
        event EventHandler<ActionTableEventArgs> Executed;
    }
}