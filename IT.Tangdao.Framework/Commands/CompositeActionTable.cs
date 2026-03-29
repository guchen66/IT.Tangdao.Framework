using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.EventArg;
using System.Threading;

namespace IT.Tangdao.Framework.Commands
{
    namespace IT.Tangdao.Framework.Commands
    {
        /// <summary>
        /// 复合委托注册表类，用于组合多个IActionTable实例
        /// </summary>
        /// <remarks>
        /// 实现了IActionTable接口，内部管理多个IActionTable实例，支持批量操作
        /// 类似于Prism框架的CompositeCommand类，但针对IActionTable接口设计
        /// 采用密封类设计，防止继承
        /// </remarks>
        public sealed class CompositeActionTable : IActionTable, IActionTableHandler
        {
            /// <summary>
            /// 用于存储多个IActionTable实例的队列
            /// </summary>
            private readonly Queue<IActionTable> _actionTables = new Queue<IActionTable>();

            /// <summary>
            /// 指示是否激活复合操作的标志
            /// </summary>
            private readonly bool _actionActive;

            /// <summary>
            /// 用于在特定线程上执行命令的同步上下文
            /// </summary>
            private readonly SynchronizationContext _synchronizationContext;

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
            /// 无参构造函数，默认激活复合操作
            /// </summary>
            public CompositeActionTable() : this(true)
            {
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="actionActive">是否激活复合操作</param>
            /// <param name="synchronizationContext">用于执行命令的同步上下文，默认为当前线程的上下文</param>
            public CompositeActionTable(bool actionActive)
            {
                _actionActive = actionActive;
                _synchronizationContext = SynchronizationContext.Current;
            }

            /// <summary>
            /// 触发注册委托事件
            /// </summary>
            /// <param name="key">委托的唯一标识符</param>
            /// <param name="priority">委托优先级</param>
            private void OnRegistered(string key, TaskPriority priority)
            {
                // 如果设置了同步上下文，则在该上下文中触发事件
                if (_synchronizationContext != null)
                {
                    _synchronizationContext.Post(state =>
                    {
                        Registered?.Invoke(this, new ActionTableEventArgs(ActionTableEventType.Registered, key, priority));
                    }, null);
                }
                else
                {
                    Registered?.Invoke(this, new ActionTableEventArgs(ActionTableEventType.Registered, key, priority));
                }
            }

            /// <summary>
            /// 触发移除委托事件
            /// </summary>
            /// <param name="key">委托的唯一标识符</param>
            /// <param name="priority">委托优先级</param>
            private void OnUnregistered(string key, TaskPriority priority)
            {
                // 如果设置了同步上下文，则在该上下文中触发事件
                if (_synchronizationContext != null)
                {
                    _synchronizationContext.Post(state =>
                    {
                        Unregistered?.Invoke(this, new ActionTableEventArgs(ActionTableEventType.Unregistered, key, priority));
                    }, null);
                }
                else
                {
                    Unregistered?.Invoke(this, new ActionTableEventArgs(ActionTableEventType.Unregistered, key, priority));
                }
            }

            /// <summary>
            /// 触发执行委托事件
            /// </summary>
            /// <param name="key">委托的唯一标识符</param>
            /// <param name="priority">委托优先级</param>
            private void OnExecuting(string key, TaskPriority priority)
            {
                // 如果设置了同步上下文，则在该上下文中触发事件
                if (_synchronizationContext != null)
                {
                    _synchronizationContext.Post(state =>
                    {
                        Executing?.Invoke(this, new ActionTableEventArgs(ActionTableEventType.Executing, key, priority));
                    }, null);
                }
                else
                {
                    Executing?.Invoke(this, new ActionTableEventArgs(ActionTableEventType.Executing, key, priority));
                }
            }

            /// <summary>
            /// 触发执行后事件
            /// </summary>
            /// <param name="key">委托的唯一标识符</param>
            /// <param name="priority">委托优先级</param>
            private void OnExecuted(string key, TaskPriority priority)
            {
                // 如果设置了同步上下文，则在该上下文中触发事件
                if (_synchronizationContext != null)
                {
                    _synchronizationContext.Post(state =>
                    {
                        Executed?.Invoke(this, new ActionTableEventArgs(ActionTableEventType.Executed, key, priority));
                    }, null);
                }
                else
                {
                    Executed?.Invoke(this, new ActionTableEventArgs(ActionTableEventType.Executed, key, priority));
                }
            }

            /// <summary>
            /// 添加一个IActionTable实例到复合表中
            /// </summary>
            /// <param name="actionTable">要添加的IActionTable实例</param>
            /// <exception cref="ArgumentNullException">当actionTable为null时抛出</exception>
            public void AddActionTable(IActionTable actionTable)
            {
                if (actionTable == null)
                {
                    throw new ArgumentNullException(nameof(actionTable));
                }

                // 如果子表实现了IActionTableHandler，订阅其事件
                if (actionTable is IActionTableHandler handler)
                {
                    handler.Registered += (sender, e) => OnRegistered(e.Key, e.Priority);
                    handler.Unregistered += (sender, e) => OnUnregistered(e.Key, e.Priority);
                    handler.Executing += (sender, e) => OnExecuting(e.Key, e.Priority);
                    handler.Executed += (sender, e) => OnExecuted(e.Key, e.Priority);
                }

                _actionTables.Enqueue(actionTable);
            }

            /// <summary>
            /// 移除所有IActionTable实例
            /// </summary>
            public void ClearActionTables()
            {
                _actionTables.Clear();
            }

            /// <summary>
            /// 获取当前复合表中包含的IActionTable实例数量
            /// </summary>
            public int ActionTableCount => _actionTables.Count;

            /// <summary>
            /// 注册一个无参数的命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <param name="action">要注册的无参数委托</param>
            /// <param name="priority">命令优先级</param>
            /// <remarks>
            /// 将命令注册到所有内部IActionTable实例中
            /// </remarks>
            public void Register(string key, Action action, TaskPriority priority = TaskPriority.Normal)
            {
                if (!_actionActive)
                    return;

                foreach (var actionTable in _actionTables)
                {
                    actionTable.Register(key, action, priority);
                }
            }

            /// <summary>
            /// 注册一个带ActionResult参数的命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <param name="action">要注册的带ActionResult参数的委托</param>
            /// <param name="priority">命令优先级</param>
            /// <remarks>
            /// 将命令注册到所有内部IActionTable实例中
            /// </remarks>
            public void Register(string key, Action<ActionResult> action, TaskPriority priority = TaskPriority.Normal)
            {
                if (!_actionActive)
                    return;

                foreach (var actionTable in _actionTables)
                {
                    actionTable.Register(key, action, priority);
                }
            }

            /// <summary>
            /// 根据命令键获取无参数的命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <returns>返回最后一个注册的IActionTable实例中的命令处理程序</returns>
            /// <remarks>
            /// 按照添加顺序，返回最后一个注册的IActionTable实例中的命令处理程序
            /// 如果没有找到，返回null
            /// </remarks>
            public Action GetHandler(string key)
            {
                // 从最后一个添加的IActionTable开始查找，返回第一个找到的处理程序
                return _actionTables.LastOrDefault(table => table.IsHandlerRegistered(key))?.GetHandler(key);
            }

            /// <summary>
            /// 根据命令键获取带ActionResult参数的命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <returns>返回最后一个注册的IActionTable实例中的命令处理程序</returns>
            /// <remarks>
            /// 按照添加顺序，返回最后一个注册的IActionTable实例中的命令处理程序
            /// 如果没有找到，返回null
            /// </remarks>
            public Action<ActionResult> GetResultHandler(string key)
            {
                // 从最后一个添加的IActionTable开始查找，返回第一个找到的处理程序
                return _actionTables.LastOrDefault(table => table.IsResultHandlerRegistered(key))?.GetResultHandler(key);
            }

            /// <summary>
            /// 移除指定键的无参数命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <returns>如果任何一个IActionTable成功移除了处理程序，则返回true</returns>
            /// <remarks>
            /// 移除所有内部IActionTable实例中对应的命令处理程序
            /// </remarks>
            public bool UnregisterHandler(string key)
            {
                bool result = false;
                foreach (var actionTable in _actionTables)
                {
                    if (actionTable.UnregisterHandler(key))
                    {
                        result = true;
                    }
                }
                return result;
            }

            /// <summary>
            /// 移除指定键的带ActionResult参数的命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <returns>如果任何一个IActionTable成功移除了处理程序，则返回true</returns>
            /// <remarks>
            /// 移除所有内部IActionTable实例中对应的命令处理程序
            /// </remarks>
            public bool UnregisterResultHandler(string key)
            {
                bool result = false;
                foreach (var actionTable in _actionTables)
                {
                    if (actionTable.UnregisterResultHandler(key))
                    {
                        result = true;
                    }
                }
                return result;
            }

            /// <summary>
            /// 检查是否已注册指定键的无参数命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <returns>如果任何一个IActionTable已注册该处理程序，则返回true</returns>
            /// <remarks>
            /// 检查所有内部IActionTable实例，只要有一个已注册该处理程序，就返回true
            /// </remarks>
            public bool IsHandlerRegistered(string key)
            {
                return _actionTables.Any(table => table.IsHandlerRegistered(key));
            }

            /// <summary>
            /// 检查是否已注册指定键的带ActionResult参数的命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <returns>如果任何一个IActionTable已注册该处理程序，则返回true</returns>
            /// <remarks>
            /// 检查所有内部IActionTable实例，只要有一个已注册该处理程序，就返回true
            /// </remarks>
            public bool IsResultHandlerRegistered(string key)
            {
                return _actionTables.Any(table => table.IsResultHandlerRegistered(key));
            }

            /// <summary>
            /// 获取所有注册的命令信息
            /// </summary>
            /// <returns>返回所有内部IActionTable实例中注册的命令信息的合并结果</returns>
            /// <remarks>
            /// 合并所有内部IActionTable实例的命令信息，相同key的命令信息保留最后一个添加的IActionTable实例中的信息
            /// </remarks>
            public IReadOnlyDictionary<string, IActionInfo> GetActionInfo()
            {
                var result = new Dictionary<string, IActionInfo>();
                foreach (var actionTable in _actionTables)
                {
                    var actionInfos = actionTable.GetActionInfo();
                    foreach (var kvp in actionInfos)
                    {
                        // 后面添加的IActionTable实例中的命令信息会覆盖前面的
                        result[kvp.Key] = kvp.Value;
                    }
                }
                return result;
            }

            /// <summary>
            /// 执行指定键的无参数命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <remarks>
            /// 执行最后一个添加的IActionTable实例中注册的命令处理程序
            /// 触发执行前后事件
            /// </remarks>
            public void Execute(string key)
            {
                if (_actionActive)
                {
                    // 查找最后一个注册了该命令的IActionTable
                    var targetTable = _actionTables.LastOrDefault(table => table.IsHandlerRegistered(key));
                    if (targetTable != null)
                    {
                        // 获取命令优先级
                        var priority = targetTable.GetActionInfo().TryGetValue(key, out var info) ? info.Priority : TaskPriority.Normal;

                        // 触发执行前事件
                        OnExecuting(key, priority);

                        // 执行命令
                        targetTable.Execute(key);

                        // 触发执行后事件
                        OnExecuted(key, priority);
                    }
                }
            }

            /// <summary>
            /// 执行指定键的带ActionResult参数的命令处理程序
            /// </summary>
            /// <param name="key">命令的唯一标识符</param>
            /// <param name="result">传递给命令处理程序的ActionResult实例</param>
            /// <remarks>
            /// 执行最后一个添加的IActionTable实例中注册的命令处理程序
            /// 触发执行前后事件
            /// </remarks>
            public void Execute(string key, ActionResult result)
            {
                if (_actionActive)
                {
                    // 查找最后一个注册了该命令的IActionTable
                    var targetTable = _actionTables.LastOrDefault(table => table.IsResultHandlerRegistered(key));
                    if (targetTable != null)
                    {
                        // 获取命令优先级
                        var priority = targetTable.GetActionInfo().TryGetValue(key, out var info) ? info.Priority : TaskPriority.Normal;

                        // 触发执行前事件
                        OnExecuting(key, priority);

                        // 执行命令
                        targetTable.Execute(key, result);

                        // 触发执行后事件
                        OnExecuted(key, priority);
                    }
                }
            }
        }
    }
}