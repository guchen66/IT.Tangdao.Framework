using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.Commands
{
    /// <summary>
    /// 委托注册表的抽象基类
    /// </summary>
    public abstract class ActionTableBase : IActionTable, IActionActive
    {
        private bool _isActive = true;

        /// <summary>
        /// 获取或设置是否激活
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnActiveChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 当激活状态改变时发生的事件
        /// </summary>
        public event EventHandler ActiveChanged;

        /// <summary>
        /// 注册委托时发生的事件
        /// </summary>
        public event EventHandler<ActionTableEventArgs> Registered;

        /// <summary>
        /// 移除委托时发生的事件
        /// </summary>
        public event EventHandler<ActionTableEventArgs> Unregistered;

        /// <summary>
        /// 委托执行时发生的事件
        /// </summary>
        public event EventHandler<ActionTableEventArgs> Executing;

        /// <summary>
        /// 委托执行后发生的事件
        /// </summary>
        public event EventHandler<ActionTableEventArgs> Executed;

        /// <summary>
        /// 注册一个无参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="action">要注册的无参数委托</param>
        public abstract void Register(string key, Action action, TaskPriority priority = TaskPriority.Normal);

        /// <summary>
        /// 注册一个带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="action">要注册的带ActionResult参数的委托</param>
        public abstract void Register(string key, Action<ActionResult> action, TaskPriority priority = TaskPriority.Normal);

        /// <summary>
        /// 根据委托键获取无参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public abstract Action GetHandler(string key);

        /// <summary>
        /// 根据委托键获取带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public abstract Action<ActionResult> GetResultHandler(string key);

        /// <summary>
        /// 移除指定键的无参数委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public abstract bool UnregisterHandler(string key);

        /// <summary>
        /// 移除指定键的带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public abstract bool UnregisterResultHandler(string key);

        /// <summary>
        /// 检查是否已注册指定键的无参数委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public abstract bool IsHandlerRegistered(string key);

        /// <summary>
        /// 检查是否已注册指定键的带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public abstract bool IsResultHandlerRegistered(string key);

        /// <summary>
        /// 获取快照信息
        /// </summary>
        /// <returns></returns>
        public abstract IReadOnlyDictionary<string, IActionInfo> GetActionInfo();

        /// <summary>
        /// 执行指定键的无参数委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        public abstract void Execute(string key);

        /// <summary>
        /// 执行指定键的带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="result">传递给委托处理程序的ActionResult实例</param>
        public abstract void Execute(string key, ActionResult result);

        /// <summary>
        /// 触发激活状态改变事件
        /// </summary>
        /// <param name="e">事件参数</param>
        protected virtual void OnActiveChanged(EventArgs e)
        {
            ActiveChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 触发注册委托事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="priority">委托优先级</param>
        protected virtual void OnRegistered(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            Registered?.Invoke(this, new ActionTableEventArgs(eventType, key, priority));
        }

        /// <summary>
        /// 触发移除委托事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="priority">委托优先级</param>
        protected virtual void OnUnregistered(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            Unregistered?.Invoke(this, new ActionTableEventArgs(eventType, key, priority));
        }

        /// <summary>
        /// 触发执行委托事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="priority">委托优先级</param>
        protected virtual void OnExecuting(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            Executing?.Invoke(this, new ActionTableEventArgs(eventType, key, priority));
        }

        /// <summary>
        /// 触发执行后事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="priority">委托优先级</param>
        protected virtual void OnExecuted(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            Executed?.Invoke(this, new ActionTableEventArgs(eventType, key, priority));
        }
    }
}