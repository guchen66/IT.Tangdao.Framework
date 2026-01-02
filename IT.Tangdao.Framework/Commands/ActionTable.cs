using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private readonly ConcurrentDictionary<string, Action> _map = new ConcurrentDictionary<string, Action>();

        /// <summary>
        /// 存储带ActionResult参数委托的并发字典
        /// </summary>
        private readonly ConcurrentDictionary<string, Action<ActionResult>> _mapWithArgs = new ConcurrentDictionary<string, Action<ActionResult>>();

        /// <summary>
        /// 注册一个无参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <param name="action">要注册的无参数委托</param>
        /// <remarks>
        /// 如果key已存在，则替换为新的action
        /// </remarks>
        public void Register(string key, Action action)
        {
            _map.AddOrUpdate(key, action, (k, oldValue) => action);
        }

        /// <summary>
        /// 注册一个带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <param name="action">要注册的带ActionResult参数的委托</param>
        /// <remarks>
        /// 如果key已存在，则替换为新的action
        /// </remarks>
        public void Register(string key, Action<ActionResult> action)
        {
            _mapWithArgs.AddOrUpdate(key, action, (k, oldValue) => action);
        }

        /// <summary>
        /// 根据命令键获取无参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public Action GetHandler(string key)
        {
            return _map.TryGetValue(key, out var cmd) ? cmd : null;
        }

        /// <summary>
        /// 根据命令键获取带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        public Action<ActionResult> GetResultHandler(string key)
        {
            return _mapWithArgs.TryGetValue(key, out var cmd) ? cmd : null;
        }

        /// <summary>
        /// 移除指定键的无参数命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public bool UnregisterHandler(string key)
        {
            return _map.TryRemove(key, out _);
        }

        /// <summary>
        /// 移除指定键的带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        public bool UnregisterResultHandler(string key)
        {
            return _mapWithArgs.TryRemove(key, out _);
        }

        /// <summary>
        /// 检查是否已注册指定键的无参数命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public bool IsHandlerRegistered(string key)
        {
            return _map.ContainsKey(key);
        }

        /// <summary>
        /// 检查是否已注册指定键的带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        public bool IsResultHandlerRegistered(string key)
        {
            return _mapWithArgs.ContainsKey(key);
        }
    }
}