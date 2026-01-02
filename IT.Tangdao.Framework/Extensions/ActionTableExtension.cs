using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Commands;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.DaoTasks;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// IActionTable的扩展方法类，提供命令处理程序的异步执行功能
    /// </summary>
    /// <remarks>
    /// 封装了通过TangdaoTaskScheduler执行命令处理程序的逻辑
    /// 支持无参数和带ActionResult参数的命令处理程序
    /// 允许指定任务执行的线程亲和性
    /// </remarks>
    public static class HandlerTableExtension
    {
        /// <summary>
        /// 异步执行指定键的无参数命令处理程序
        /// </summary>
        /// <param name="table">IActionTable实例</param>
        /// <param name="key">命令的唯一标识符</param>
        /// <param name="affinity">任务执行的线程亲和性，默认为自动选择</param>
        /// <remarks>
        /// 通过TangdaoTaskScheduler调度任务执行
        /// 如果指定键的处理程序不存在，则不执行任何操作
        /// </remarks>
        public static void Execute(this IActionTable table, string key, TaskThreadType affinity = TaskThreadType.Auto)
        {
            TangdaoTaskScheduler.Execute(_ =>
            {
                table.GetHandler(key)?.Invoke();
            }, affinity);
        }

        /// <summary>
        /// 异步执行指定键的带ActionResult参数的命令处理程序
        /// </summary>
        /// <param name="table">IActionTable实例</param>
        /// <param name="key">命令的唯一标识符</param>
        /// <param name="handlerResult">要传递给处理程序的ActionResult实例</param>
        /// <param name="affinity">任务执行的线程亲和性，默认为自动选择</param>
        /// <remarks>
        /// 通过TangdaoTaskScheduler调度任务执行
        /// 如果指定键的处理程序不存在，则不执行任何操作
        /// </remarks>
        public static void Execute(this IActionTable table, string key, ActionResult handlerResult, TaskThreadType affinity = TaskThreadType.Auto)
        {
            TangdaoTaskScheduler.Execute(_ =>
            {
                table.GetResultHandler(key)?.Invoke(handlerResult);
            }, affinity);
        }
    }
}