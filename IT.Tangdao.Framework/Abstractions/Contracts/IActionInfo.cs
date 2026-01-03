using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Commands;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    public interface IActionInfo
    {
        /// <summary>
        /// 命令委托
        /// </summary>
        Action Action { get; }

        /// <summary>
        /// 命令委托
        /// </summary>
        Action<ActionResult> ActionResult { get; }

        /// <summary>
        /// 命令优先级
        /// </summary>
        TaskPriority Priority { get; }
    }
}