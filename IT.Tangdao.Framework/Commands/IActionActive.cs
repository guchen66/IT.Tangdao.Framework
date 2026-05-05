using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Commands
{
    /// <summary>
    /// 提供激活/停用功能的接口
    /// </summary>
    public interface IActionActive
    {
        /// <summary>
        /// 获取或设置是否激活
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// 当激活状态改变时发生的事件
        /// </summary>
        event EventHandler ActiveChanged;
    }
}