using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Contracts;

namespace IT.Tangdao.Framework.Commands
{
    /// <summary>
    /// 委托注册表接口，用于管理和调度命令处理程序
    /// </summary>
    public interface IActionTable
    {
        /// <summary>
        /// 注册一个无参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <param name="action">要注册的无参数委托</param>
        /// <remarks>
        /// 如果key已存在，则替换为新的action
        /// </remarks>
        void Register(string key, Action action, TaskPriority priority = TaskPriority.Normal);

        /// <summary>
        /// 注册一个带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <param name="action">要注册的带ActionResult参数的委托</param>
        /// <remarks>
        /// 如果key已存在，则替换为新的action
        /// </remarks>
        void Register(string key, Action<ActionResult> action, TaskPriority priority = TaskPriority.Normal);

        /// <summary>
        /// 根据命令键获取无参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        Action GetHandler(string key);

        /// <summary>
        /// 根据命令键获取带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果找到匹配的处理程序则返回该委托，否则返回null</returns>
        Action<ActionResult> GetResultHandler(string key);

        /// <summary>
        /// 移除指定键的无参数命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        bool UnregisterHandler(string key);

        /// <summary>
        /// 移除指定键的带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果成功移除则返回true，否则返回false</returns>
        bool UnregisterResultHandler(string key);

        /// <summary>
        /// 检查是否已注册指定键的无参数命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        bool IsHandlerRegistered(string key);

        /// <summary>
        /// 检查是否已注册指定键的带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="key">命令的唯一标识符</param>
        /// <returns>如果已注册则返回true，否则返回false</returns>
        bool IsResultHandlerRegistered(string key);

        /// <summary>
        /// 获取快照信息
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<string, IActionInfo> GetActionInfo();
    }
}