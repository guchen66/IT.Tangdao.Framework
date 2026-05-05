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
    /// 委托注册表的并发实现类
    /// </summary>
    public sealed class ActionTable : ActionTableBase
    {
        /// <summary>
        /// 存储无参数委托的并发字典
        /// </summary>
        private readonly ConcurrentDictionary<string, ActionEntry> _map = new ConcurrentDictionary<string, ActionEntry>();

        /// <summary>
        /// 注册一个无参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="action">要注册的无参数委托</param>
        public override void Register(string key, Action action, TaskPriority priority = TaskPriority.Normal)
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
        public override void Register(string key, Action<ActionResult> action, TaskPriority priority = TaskPriority.Normal)
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
            OnRegistered(ActionTableEventType.Registered, key, commandEntry.Priority);
        }

        /// <summary>
        /// 根据委托键获取无参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public override Action GetHandler(string key)
        {
            return _map.TryGetValue(key, out var cmd) ? cmd.Action : () => { };
        }

        /// <summary>
        /// 根据委托键获取带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public override Action<ActionResult> GetResultHandler(string key)
        {
            return _map.TryGetValue(key, out var cmd) ? cmd.ActionResult : action => { };
        }

        /// <summary>
        /// 移除指定键的无参数委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public override bool UnregisterHandler(string key)
        {
            if (_map.TryGetValue(key, out var e) && e.Action != null)
            {
                if (_map.TryRemove(key, out _))
                {
                    // 触发移除事件
                    OnUnregistered(ActionTableEventType.Unregistered, key, e.Priority);
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
        public override bool UnregisterResultHandler(string key)
        {
            if (_map.TryGetValue(key, out var e) && e.ActionResult != null)
            {
                if (_map.TryRemove(key, out _))
                {
                    // 触发移除事件
                    OnUnregistered(ActionTableEventType.Unregistered, key, e.Priority);
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
        public override bool IsHandlerRegistered(string key)
        {
            return _map.TryGetValue(key, out var e) && e.Action != null;
        }

        /// <summary>
        /// 检查是否已注册指定键的带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public override bool IsResultHandlerRegistered(string key)
        {
            return _map.TryGetValue(key, out var e) && e.ActionResult != null;
        }

        /// <summary>
        /// 获取快照信息
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyDictionary<string, IActionInfo> GetActionInfo()
        {
            return _map.ToDictionary(kv => kv.Key, kv => (IActionInfo)kv.Value);
        }

        /// <summary>
        /// 执行指定键的无参数委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        public override void Execute(string key)
        {
            if (IsActive && _map.TryGetValue(key, out var entry) && entry.Action != null)
            {
                // 触发执行前事件
                OnExecuting(ActionTableEventType.Executing, key, entry.Priority);

                // 执行委托
                entry.Action.Invoke();

                // 触发执行后事件
                OnExecuted(ActionTableEventType.Executed, key, entry.Priority);
            }
        }

        /// <summary>
        /// 执行指定键的带ActionResult参数的委托处理程序
        /// </summary>
        /// <param name="key">委托的唯一标识符</param>
        /// <param name="result">传递给委托处理程序的ActionResult实例</param>
        public override void Execute(string key, ActionResult result)
        {
            if (IsActive && _map.TryGetValue(key, out var entry) && entry.ActionResult != null)
            {
                // 触发执行前事件
                OnExecuting(ActionTableEventType.Executing, key, entry.Priority);

                // 执行委托
                entry.ActionResult.Invoke(result);

                // 触发执行后事件
                OnExecuted(ActionTableEventType.Executed, key, entry.Priority);
            }
        }

        /// <summary>
        /// 委托条目，包含委托和优先级信息
        /// </summary>
        private class ActionEntry : IActionInfo
        {
            /// <summary>
            /// 委托
            /// </summary>
            public Action Action { get; }

            /// <summary>
            /// 带结果的委托
            /// </summary>
            public Action<ActionResult> ActionResult { get; }

            /// <summary>
            /// 委托优先级
            /// </summary>
            public TaskPriority Priority { get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="action">委托</param>
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