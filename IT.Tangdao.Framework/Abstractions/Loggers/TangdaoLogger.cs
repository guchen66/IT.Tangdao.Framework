using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.Loggers
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public class TangdaoLogger : ITangdaoLogger
    {
        private static readonly ConcurrentDictionary<Type, TangdaoLogger> Loggers = new ConcurrentDictionary<Type, TangdaoLogger>();
        private static readonly LoggerHandler LoggerHandler = new LoggerHandler();
        public static Func<Type, TangdaoLogger> LoggerFactory { get; set; }

        /// <summary>
        /// 全局日志级别，默认Info
        /// </summary>
        public static LoggerLevel GlobalLogLevel { get; set; } = LoggerLevel.Info;

        /// <summary>
        /// 全局日志输出方式，默认File | Debug
        /// </summary>
        public static LogOutputType GlobalOutputType { get; set; } = LogOutputType.File | LogOutputType.Debug;

        public static TangdaoLogger Get<TType>() => Get(typeof(TType));

        public static TangdaoLogger Get(Type type)
        {
            //TryGetValue避免两次查找，直接返回logger
            if (!Loggers.TryGetValue(type, out TangdaoLogger logger))
            {
                logger = LoggerFactory?.Invoke(type) ?? new TangdaoLogger(type);
                Loggers[type] = logger; // 或者使用 TryAdd 更安全
            }

            return logger;
        }

        private readonly Type _type;

        /// <summary>
        /// 当前日志级别，默认使用全局日志级别
        /// </summary>
        public LoggerLevel LoggerLevel { get; set; } = GlobalLogLevel;

        /// <summary>
        /// 当前日志输出方式，默认使用全局日志输出方式
        /// </summary>
        public LogOutputType OutputType { get; set; } = GlobalOutputType;

        protected TangdaoLogger(Type type) => _type = type;

        public void Fatal(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Fatal)
            {
                EnqueueLog(LoggerLevel.Fatal, message, e);
            }
        }

        public void Error(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Error)
            {
                EnqueueLog(LoggerLevel.Error, message, e);
            }
        }

        public void Warning(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Warning)
            {
                EnqueueLog(LoggerLevel.Warning, message, e);
            }
        }

        public void Info(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Info)
            {
                EnqueueLog(LoggerLevel.Info, message, e);
            }
        }

        public void Debug(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Debug)
            {
                EnqueueLog(LoggerLevel.Debug, message, e);
            }
        }

        /// <summary>
        /// 异步记录致命级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public Task FatalAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Fatal)
            {
                EnqueueLog(LoggerLevel.Fatal, message, e);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步记录错误级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public Task ErrorAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Error)
            {
                EnqueueLog(LoggerLevel.Error, message, e);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步记录警告级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public Task WarningAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Warning)
            {
                EnqueueLog(LoggerLevel.Warning, message, e);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步记录信息级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public Task InfoAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Info)
            {
                EnqueueLog(LoggerLevel.Info, message, e);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步记录调试级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public Task DebugAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Debug)
            {
                EnqueueLog(LoggerLevel.Debug, message, e);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 将日志加入队列
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        private void EnqueueLog(LoggerLevel level, string message, Exception e)
        {
            var logItem = new LoggerHandler.LogItem
            {
                Level = level,
                Message = message,
                Exception = e,
                Type = _type,
                OutputType = OutputType
            };
            LoggerHandler.EnqueueLog(logItem);
        }
    }
}