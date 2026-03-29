using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.Commands
{
    /// <summary>
    /// 委托注册表的并发实现类
    /// </summary>
    public sealed class ActionTable : IActionTable
    {
        /// <summary>
        /// 存储无参数委托的并发字典
        /// </summary>
        private readonly ConcurrentDictionary<string, ActionEntry> _map = new ConcurrentDictionary<string, ActionEntry>();

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
        /// 微弱执行后发生的事件
        /// </summary>
        public event EventHandler<ActionTableEventArgs> Executed;

        /// <summary>
        /// 注册一个无参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="action">要注册的无参数委托</param>
        public void Register(string key, Action action, TaskPriority priority = TaskPriority.Normal)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
            ActionEntry commandEntry = new ActionEntry(action, priority);
            RegisterInternal(key, commandEntry);
        }

        /// <summary>
        /// 注册一个带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="action">要注册的带ActionResult参数的委托</param>
        public void Register(string key, Action<ActionResult> action, TaskPriority priority = TaskPriority.Normal)
        {
            ActionEntry commandEntry = new ActionEntry(action, priority);
            RegisterInternal(key, commandEntry);
        }

        private void RegisterInternal(string key, ActionEntry commandEntry)
        {
            _map.AddOrUpdate(key, commandEntry, (k, existing) =>
            {
                /* 同优先级 → 无论高低，直接拒绝 */
                if (existing.Priority == commandEntry.Priority)
                    throw new InvalidOperationException(
                        $"Key '{key}' 已存在，且任务优先级相同，请勿重复注册.");

                /* 低优先级试图覆盖高优先级 → 也拒绝 */
                if (commandEntry.Priority < existing.Priority)
                    throw new InvalidOperationException(
                        $"Key '{key}' exists with higher priority {existing.Priority}.");

                /* 高优先级覆盖低优先级 → 允许 */
                return commandEntry;
            });

            // 触发注册事件
            OnRegisterChanged(ActionTableEventType.Registered, key, commandEntry.Priority);
        }

        /// <summary>
        /// 根据委托键获取无参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public Action GetHandler(string key)
        {
            return _map.TryGetValue(key, out var cmd) ? cmd.Action : () => { };
        }

        /// <summary>
        /// 根据委托键获取带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public Action<ActionResult> GetResultHandler(string key)
        {
            return _map.TryGetValue(key, out var cmd) ? cmd.ActionResult : action => { };
        }

        /// <summary>
        /// 移除指定键的无参数委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public bool UnregisterHandler(string key)
        {
            if (_map.TryGetValue(key, out var e) && e.Action != null)
            {
                if (_map.TryRemove(key, out _))
                {
                    // 触发移除事件
                    OnUnregisterChanged(ActionTableEventType.Unregistered, key, e.Priority);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除指定键的带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public bool UnregisterResultHandler(string key)
        {
            if (_map.TryGetValue(key, out var e) && e.ActionResult != null)
            {
                if (_map.TryRemove(key, out _))
                {
                    // 触发移除事件
                    OnUnregisterChanged(ActionTableEventType.Unregistered, key, e.Priority);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否已注册指定键的无参数委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public bool IsHandlerRegistered(string key)
        {
            return _map.TryGetValue(key, out var e) && e.Action != null;
        }

        /// <summary>
        /// 检查是否已注册指定键的带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public bool IsResultHandlerRegistered(string key)
        {
            return _map.TryGetValue(key, out var e) && e.ActionResult != null;
        }

        /// <summary>
        /// 获取快照信息
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<string, IActionInfo> IActionTable.GetActionInfo()
        {
            return _map.ToDictionary(kv => kv.Key, kv => (IActionInfo)kv.Value);
        }

        /// <summary>
        /// 执行指定键的无参数委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        public void Execute(string key)
        {
            if (_map.TryGetValue(key, out var entry) && entry.Action != null)
            {
                // 触发执行前事件
                OnExecutingChanged(ActionTableEventType.Executing, key, entry.Priority);

                // 执行委托
                entry.Action.Invoke();

                // 触发执行后事件
                OnExecutedChanged(ActionTableEventType.Executed, key, entry.Priority);
            }
        }

        /// <summary>
        /// 执行指定键的带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="result">传递给委托处理程序的ActionResult实例</param>
        public void Execute(string key, ActionResult result)
        {
            if (_map.TryGetValue(key, out var entry) && entry.ActionResult != null)
            {
                // 触发执行前事件
                OnExecutingChanged(ActionTableEventType.Executing, key, entry.Priority);

                // 执行委托
                entry.ActionResult.Invoke(result);

                // 触发执行后事件
                OnExecutedChanged(ActionTableEventType.Executed, key, entry.Priority);
            }
        }

        /// <summary>
        /// 触发注册委托事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="priority">委托优先级</param>
        private void OnRegisterChanged(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            Registered?.Invoke(this, new ActionTableEventArgs(eventType, key, priority));
        }

        /// <summary>
        /// 触发移除委托事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="priority">委托优先级</param>
        private void OnUnregisterChanged(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            Unregistered?.Invoke(this, new ActionTableEventArgs(eventType, key, priority));
        }

        /// <summary>
        /// 触发执行委托事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="priority">委托优先级</param>
        private void OnExecutingChanged(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            Executing?.Invoke(this, new ActionTableEventArgs(eventType, key, priority));
        }

        /// <summary>
        /// 触发注册事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="priority">委托优先级</param>
        private void OnExecutedChanged(ActionTableEventType eventType, string key, TaskPriority priority)
        {
            Executed?.Invoke(this, new ActionTableEventArgs(eventType, key, priority));
        }

        /// <summary>
        /// 委托条目，包含委托和优先级信息
        /// </summary>
        private class ActionEntry : IActionInfo
        {
            /// <summary>
            /// 委托委托
            /// </summary>
            public Action Action { get; }

            /// <summary>
            /// 委托委托
            /// </summary>
            public Action<ActionResult> ActionResult { get; }

            /// <summary>
            /// 委托优先级
            /// </summary>
            public TaskPriority Priority { get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="action">委托委托</param>
            /// <param name="priority">委托优先级</param>
            public ActionEntry(Action action, TaskPriority priority)
            {
                Action = action;
                Priority = priority;
            }

            public ActionEntry(Action<ActionResult> actionResult, TaskPriority priority)
            {
                ActionResult = actionResult;
                Priority = priority;
            }
        }
    }
}