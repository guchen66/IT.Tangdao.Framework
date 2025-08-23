using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEvents
{
    public class DaoEventAggregator : IDaoEventAggregator
    {
        private class WeakHandler
        {
            internal readonly WeakReference _targetWeakRef;  // 对目标对象的弱引用
            internal readonly MethodInfo _method;
            public bool IsAsync { get; }

            public WeakHandler(Delegate handler, bool isAsync)
            {
                _method = handler.Method;
                _targetWeakRef = new WeakReference(handler.Target); // 存储目标对象的弱引用
                IsAsync = isAsync;
            }

            public bool TryGetDelegate(Type delegateType, out Delegate @delegate)
            {
                @delegate = null;
                object target = null;

                // 对于实例方法，检查目标是否存活
                if (!_method.IsStatic)
                {
                    target = _targetWeakRef.Target;
                    if (target == null) return false; // 目标已被GC回收
                }

                try
                {
                    @delegate = _method.IsStatic
                        ? Delegate.CreateDelegate(delegateType, _method)
                        : Delegate.CreateDelegate(delegateType, target, _method);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 用于缓存空事件的静态字典，避免重复创建空事件对象浪费资源
        /// Key: 事件类型 Type
        /// Value: 对应类型的空事件对象
        /// </summary>
        private static readonly ConcurrentDictionary<Type, DaoEventBase> _emptyEvents = new ConcurrentDictionary<Type, DaoEventBase>();

        private readonly ConcurrentDictionary<Type, List<WeakHandler>> _weakEventHandlers = new ConcurrentDictionary<Type, List<WeakHandler>>();
        private readonly ConcurrentDictionary<Type, List<WeakHandler>> _weakAsyncEventHandlers = new ConcurrentDictionary<Type, List<WeakHandler>>();
        private readonly object _cleanupLock = new object();
        private readonly object _operationLock = new object();

        public void Subscribe<T>(DaoEventHandler<T> handler) where T : DaoEventBase, new()
        {
            AddHandler(handler, false);
        }

        public void SubscribeAsync<T>(DaoAsyncEventHandler<T> handler) where T : DaoEventBase, new()
        {
            AddHandler(handler, true);
        }

        private void AddHandler(Delegate handler, bool isAsync)
        {
            lock (_operationLock)
            {
                CleanupCollectedHandlers();

                var weakHandler = new WeakHandler(handler, isAsync);
                var eventType = handler.GetMethodInfo().GetParameters()[0].ParameterType;

                // 根据是否异步选择正确的字典
                var targetDictionary = isAsync ? _weakAsyncEventHandlers : _weakEventHandlers;

                targetDictionary.AddOrUpdate(
                    eventType,
                    _ => new List<WeakHandler> { weakHandler },
                    (_, list) => { list.Add(weakHandler); return list; }
                );
            }
        }

        private void CleanupCollectedHandlers()
        {
            // 清理同步处理器
            CleanupHandlers(_weakEventHandlers);

            // 清理异步处理器
            CleanupHandlers(_weakAsyncEventHandlers);
        }

        private void CleanupHandlers(ConcurrentDictionary<Type, List<WeakHandler>> handlersDict)
        {
            foreach (var kvp in handlersDict)
            {
                var eventType = kvp.Key;
                var handlers = kvp.Value;

                // 获取当前事件类型对应的委托类型
                Type delegateType = handlers.FirstOrDefault()?.IsAsync == true
                    ? typeof(DaoAsyncEventHandler<>).MakeGenericType(eventType)
                    : typeof(DaoEventHandler<>).MakeGenericType(eventType);

                lock (_cleanupLock)
                {
                    var aliveHandlers = handlers
                        .Where(wh => wh.TryGetDelegate(delegateType, out _))
                        .ToList();

                    if (aliveHandlers.Count != handlers.Count)
                    {
                        handlersDict[eventType] = aliveHandlers;
                    }
                }
            }
        }

        public void Publish<T>(T @event = null) where T : DaoEventBase, new()
        {
            var eventObj = @event ?? GetEmptyEvent<T>();

            if (!_weakEventHandlers.TryGetValue(typeof(T), out var weakHandlers))
                return;

            var aliveHandlers = new List<DaoEventHandler<T>>();

            foreach (var weakHandler in weakHandlers)
            {
                if (weakHandler.TryGetDelegate(typeof(DaoEventHandler<T>), out var handler) &&
                    handler is DaoEventHandler<T> typedHandler)
                {
                    aliveHandlers.Add(typedHandler);
                }
            }

            // 执行所有存活处理器
            foreach (var handler in aliveHandlers)
            {
                try
                {
                    handler(eventObj);
                }
                catch (Exception ex)
                {
                    // 记录错误但继续执行其他处理器
                    Debug.WriteLine($"Event handler failed: {ex}");
                }
            }

            // 自动清理失效的处理器
            if (aliveHandlers.Count < weakHandlers.Count)
            {
                CleanupHandlers(_weakEventHandlers);
            }
        }

        /// <summary>
        /// 异步发布事件
        /// </summary>
        public async Task PublishAsync<T>(T @event = null) where T : DaoEventBase, new()
        {
            var eventObj = @event ?? GetEmptyEvent<T>();

            if (!_weakAsyncEventHandlers.TryGetValue(typeof(T), out var weakHandlers))
                return;

            var tasks = new List<Task>();
            var deadHandlerIndices = new List<int>();

            // 第一阶段：准备异步任务
            for (int i = 0; i < weakHandlers.Count; i++)
            {
                if (weakHandlers[i].TryGetDelegate(typeof(DaoAsyncEventHandler<T>), out var handler) &&
                    handler is DaoAsyncEventHandler<T> typedHandler)
                {
                    tasks.Add(typedHandler(eventObj));
                }
                else
                {
                    deadHandlerIndices.Add(i);
                }
            }

            // 第二阶段：执行异步处理器
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            // 第三阶段：清理失效处理器
            if (deadHandlerIndices.Count > 0)
            {
                lock (_cleanupLock)
                {
                    // 从后往前移除，避免索引错位
                    foreach (var index in deadHandlerIndices.OrderByDescending(i => i))
                    {
                        if (index < weakHandlers.Count)
                        {
                            weakHandlers.RemoveAt(index);
                        }
                    }

                    if (weakHandlers.Count == 0)
                    {
                        _weakAsyncEventHandlers.TryRemove(typeof(T), out _);
                    }
                }
            }
            if (deadHandlerIndices.Count > 0)
            {
                CleanupHandlers(_weakAsyncEventHandlers);
            }
        }

        /// <summary>
        /// 取消订阅同步事件
        /// </summary>
        public void UnSubscribe<T>(DaoEventHandler<T> handler) where T : DaoEventBase, new()
        {
            if (handler == null) return;

            if (_weakEventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                lock (_cleanupLock)
                {
                    // 找出所有匹配的处理器（比较方法签名和目标对象）
                    var toRemove = handlers
                        .Where(wh => !wh.IsAsync &&
                               wh._method == handler.Method &&
                               (wh._targetWeakRef?.Target == handler.Target ||
                                (wh._method.IsStatic && handler.Target == null)))
                        .ToList();

                    // 执行移除
                    foreach (var item in toRemove)
                    {
                        handlers.Remove(item);
                    }

                    // 如果列表为空，移除整个事件类型
                    if (handlers.Count == 0)
                    {
                        _weakEventHandlers.TryRemove(typeof(T), out _);
                    }
                }
            }
        }

        /// <summary>
        /// 取消订阅异步事件
        /// </summary>
        public void UnSubscribeAsync<T>(DaoAsyncEventHandler<T> handler) where T : DaoEventBase, new()
        {
            if (handler == null) return;

            if (_weakAsyncEventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                lock (_cleanupLock)
                {
                    // 比较方法签名和目标对象
                    var toRemove = handlers
                        .Where(wh => wh.IsAsync &&
                               wh._method == handler.Method &&
                               (wh._targetWeakRef?.Target == handler.Target ||
                                (wh._method.IsStatic && handler.Target == null)))
                        .ToList();

                    foreach (var item in toRemove)
                    {
                        handlers.Remove(item);
                    }

                    if (handlers.Count == 0)
                    {
                        _weakAsyncEventHandlers.TryRemove(typeof(T), out _);
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定类型的空事件对象
        /// 使用缓存避免重复创建
        /// </summary>
        /// <typeparam name="T">事件类型，必须继承自DaoEventBase并有无参构造函数</typeparam>
        /// <returns>对应类型的空事件对象</returns>
        private T GetEmptyEvent<T>() where T : DaoEventBase, new()
        {
            // 从缓存中获取或创建空事件对象
            return (T)_emptyEvents.GetOrAdd(typeof(T), _ => new T());
        }
    }
}