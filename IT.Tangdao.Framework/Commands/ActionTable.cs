using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Contracts;

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
        /// 注册一个无参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <param name="action">要注册的无参数委托</param>
        public void Register(string key, Action action, TaskPriority priority = TaskPriority.Normal)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
            ActionEntry commandEntry = new ActionEntry(action, priority);
            RegisterInternal(key, commandEntry);
        }

        /// <summary>
        /// 注册一个带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
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
        }

        /// <summary>
        /// 根据命令键获取无参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public Action GetHandler(string key)
        {
            return _map.TryGetValue(key, out var cmd) ? cmd.Action : () => { };
        }

        /// <summary>
        /// 根据命令键获取带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public Action<ActionResult> GetResultHandler(string key)
        {
            return _map.TryGetValue(key, out var cmd) ? cmd.ActionResult : action => { };
        }

        /// <summary>
        /// 移除指定键的无参数命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public bool UnregisterHandler(string key)
        {
            return _map.TryGetValue(key, out var e) && e.Action != null && _map.TryRemove(key, out _);
        }

        /// <summary>
        /// 移除指定键的带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public bool UnregisterResultHandler(string key)
        {
            return _map.TryGetValue(key, out var e) && e.ActionResult != null && _map.TryRemove(key, out _);
        }

        /// <summary>
        /// 检查是否已注册指定键的无参数命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public bool IsHandlerRegistered(string key)
        {
            return _map.TryGetValue(key, out var e) && e.Action != null;
        }

        /// <summary>
        /// 检查是否已注册指定键的带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public bool IsResultHandlerRegistered(string key)
        {
            return _map.TryGetValue(key, out var e) && e.ActionResult != null;
        }

        /// <summary>
        /// 获取快照信息
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, IActionInfo> GetActionInfo()
        {
            return _map.ToDictionary(kv => kv.Key, kv => (IActionInfo)kv.Value);
        }

        /// <summary>
        /// 命令条目，包含委托和优先级信息
        /// </summary>
        private class ActionEntry : IActionInfo
        {
            /// <summary>
            /// 命令委托
            /// </summary>
            public Action Action { get; }

            /// <summary>
            /// 命令委托
            /// </summary>
            public Action<ActionResult> ActionResult { get; }

            /// <summary>
            /// 命令优先级
            /// </summary>
            public TaskPriority Priority { get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="action">命令委托</param>
            /// <param name="priority">命令优先级</param>
            /// <param name="sender">发送者信息</param>
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