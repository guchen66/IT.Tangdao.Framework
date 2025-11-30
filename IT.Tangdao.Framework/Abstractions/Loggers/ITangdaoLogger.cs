using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.Loggers
{
    public interface ITangdaoLogger
    {
        /// <summary>
        /// 获取或设置当前日志级别，低于该级别的日志将不会被记录
        /// </summary>
        LoggerLevel LoggerLevel { get; set; }

        /// <summary>
        /// 获取或设置当前日志输出方式
        /// </summary>
        LogOutputType OutputType { get; set; }

        /// <summary>
        /// 记录致命日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        void Fatal(string message, Exception e = null);

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        void Error(string message, Exception e = null);

        /// <summary>
        /// 记录经过日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        void Warning(string message, Exception e = null);

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        void Info(string message, Exception e = null);

        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        void Debug(string message, Exception e = null);

        /// <summary>
        /// 异步记录致命级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        Task FatalAsync(string message, Exception e = null);

        /// <summary>
        /// 异步记录错误级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        Task ErrorAsync(string message, Exception e = null);

        /// <summary>
        /// 异步记录警告级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        Task WarningAsync(string message, Exception e = null);

        /// <summary>
        /// 异步记录信息级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        Task InfoAsync(string message, Exception e = null);

        /// <summary>
        /// 异步记录调试级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        Task DebugAsync(string message, Exception e = null);
    }
}