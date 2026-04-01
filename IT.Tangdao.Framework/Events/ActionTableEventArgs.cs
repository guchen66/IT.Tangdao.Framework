using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Events
{
    /// <summary>
    /// 委托注册表事件参数
    /// </summary>
    public class ActionTableEventArgs : EventArgs
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public ActionTableEventType EventType { get; }

        /// <summary>
        /// 命令的唯一标识符
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 命令优先级
        /// </summary>
        public TaskPriority Priority { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">命令的唯一标识符</param>
        /// <param name="priority">命令优先级</param>
        public ActionTableEventArgs(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            EventType = eventType;
            Key = key;
            Priority = priority;
        }
    }
}