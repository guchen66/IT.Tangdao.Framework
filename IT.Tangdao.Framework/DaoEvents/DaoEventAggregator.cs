using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEvents
{
    public class DaoEventAggregator : IDaoEventAggregator
    {
        /// <summary>
        /// 用于缓存空事件的静态字典，避免重复创建空事件对象浪费资源
        /// Key: 事件类型 Type
        /// Value: 对应类型的空事件对象
        /// </summary>
        private static readonly ConcurrentDictionary<Type, DaoEventBase> _emptyEvents = new ConcurrentDictionary<Type, DaoEventBase>();

        /// <summary>
        /// 存储事件类型与对应处理器列表的字典
        /// Key: 事件类型 Type
        /// Value: 该事件类型的处理器委托列表
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<Delegate>> _eventHandlers = new ConcurrentDictionary<Type, List<Delegate>>();

        /// <summary>
        /// 订阅指定类型的事件
        /// </summary>
        /// <typeparam name="T">事件类型，必须继承自DaoEventBase并有无参构造函数</typeparam>
        /// <param name="handler">事件处理器委托</param>
        public void Subscribe<T>(DaoEventHandler<T> handler) where T : DaoEventBase, new()
        {
            // 获取或创建指定事件类型的处理器列表
            List<Delegate> handlers = _eventHandlers.GetOrAdd(typeof(T), _ => new List<Delegate>());
            // 将新处理器添加到列表中
            handlers.Add(handler);
        }

        /// <summary>
        /// 发布指定类型的事件
        /// </summary>
        /// <typeparam name="T">事件类型，必须继承自DaoEventBase并有无参构造函数</typeparam>
        /// <param name="event">要发布的事件对象，如果为null则使用空事件对象</param>
        public void Publish<T>(T @event = null) where T : DaoEventBase, new()
        {
            // 检查是否有该事件的处理器
            if (_eventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                // 如果传入的事件为null，则获取该类型的空事件对象
                var eventObj = @event ?? GetEmptyEvent<T>();
                // 遍历所有处理器并执行
                foreach (var handler in handlers)
                {
                    ((DaoEventHandler<T>)handler)(eventObj);
                }
            }
        }

        /// <summary>
        /// 取消订阅指定类型的事件
        /// </summary>
        /// <typeparam name="T">事件类型，必须继承自DaoEventBase并有无参构造函数</typeparam>
        /// <param name="handler">要移除的事件处理器委托</param>
        public void UnSubscribe<T>(DaoEventHandler<T> handler) where T : DaoEventBase, new()
        {
            var eventType = typeof(T);
            // 检查是否存在该事件的处理器列表
            if (!_eventHandlers.ContainsKey(eventType))
            {
                // 从处理器列表中移除指定的处理器
                _eventHandlers[eventType].Remove(handler);
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