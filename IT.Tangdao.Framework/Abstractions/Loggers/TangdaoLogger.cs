using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.Loggers
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public class TangdaoLogger : ITangdaoLogger
    {
        private static readonly ConcurrentDictionary<Type, TangdaoLogger> Loggers = new ConcurrentDictionary<Type, TangdaoLogger>();
        private static readonly ConcurrentDictionary<string, StreamWriter> s_fileWriters = new ConcurrentDictionary<string, StreamWriter>(StringComparer.Ordinal);
        public static Func<Type, TangdaoLogger> LoggerFactory { get; set; }

        /// <summary>
        /// 全局日志级别，默认Info
        /// </summary>
        public static LoggerLevel GlobalLogLevel { get; set; } = LoggerLevel.Info;

        /// <summary>
        /// 全局日志输出方式，默认File | Debug
        /// </summary>
        public static LogOutputType GlobalOutputType { get; set; } = LogOutputType.File | LogOutputType.Debug;

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
                Write(LoggerLevel.Fatal, message, e);
            }
        }

        public void Error(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Error)
            {
                Write(LoggerLevel.Error, message, e);
            }
        }

        public void Warning(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Warning)
            {
                Write(LoggerLevel.Warning, message, e);
            }
        }

        public void Info(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Info)
            {
                Write(LoggerLevel.Info, message, e);
            }
        }

        public void Debug(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Debug)
            {
                Write(LoggerLevel.Debug, message, e);
            }
        }

        /// <summary>
        /// 异步记录致命级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public async Task FatalAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Fatal)
            {
                await WriteAsync(LoggerLevel.Fatal, message, e);
            }
        }

        /// <summary>
        /// 异步记录错误级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public async Task ErrorAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Error)
            {
                await WriteAsync(LoggerLevel.Error, message, e);
            }
        }

        /// <summary>
        /// 异步记录警告级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public async Task WarningAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Warning)
            {
                await WriteAsync(LoggerLevel.Warning, message, e);
            }
        }

        /// <summary>
        /// 异步记录信息级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public async Task InfoAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Info)
            {
                await WriteAsync(LoggerLevel.Info, message, e);
            }
        }

        /// <summary>
        /// 异步记录调试级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        public async Task DebugAsync(string message, Exception e = null)
        {
            if (LoggerLevel <= LoggerLevel.Debug)
            {
                await WriteAsync(LoggerLevel.Debug, message, e);
            }
        }

        private void Write(LoggerLevel level, string message, Exception e)
        {
            string logLine = FormatLogLine(level, message, e);

            // 根据输出方式输出日志
            if (OutputType.HasFlag(LogOutputType.File))
            {
                var file = System.IO.Path.Combine(LogPathConfig.Root, "Tangdao.log");
                var writer = GetWriter(file);
                writer.Write(logLine);
            }

            if (OutputType.HasFlag(LogOutputType.Console))
            {
                // 根据日志级别设置控制台颜色
                ConsoleColor originalColor = Console.ForegroundColor;
                switch (level)
                {
                    case LoggerLevel.Fatal:
                    case LoggerLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case LoggerLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case LoggerLevel.Info:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case LoggerLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                }

                Console.Write(logLine);
                Console.ForegroundColor = originalColor;
            }

            if (OutputType.HasFlag(LogOutputType.Debug))
            {
                System.Diagnostics.Debug.Write(logLine);
            }
        }

        /// <summary>
        /// 异步写入日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>异步任务</returns>
        private async Task WriteAsync(LoggerLevel level, string message, Exception e)
        {
            string logLine = FormatLogLine(level, message, e);

            // 根据输出方式输出日志
            if (OutputType.HasFlag(LogOutputType.File))
            {
                var file = System.IO.Path.Combine(LogPathConfig.Root, "Tangdao.log");
                var writer = GetWriter(file);
                await writer.WriteAsync(logLine);          // 异步写入，避免阻塞
            }

            if (OutputType.HasFlag(LogOutputType.Console))
            {
                // 根据日志级别设置控制台颜色
                ConsoleColor originalColor = Console.ForegroundColor;
                switch (level)
                {
                    case LoggerLevel.Fatal:
                    case LoggerLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case LoggerLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case LoggerLevel.Info:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case LoggerLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                }

                await Console.Out.WriteAsync(logLine);
                Console.ForegroundColor = originalColor;
            }

            if (OutputType.HasFlag(LogOutputType.Debug))
            {
                System.Diagnostics.Debug.Write(logLine);
            }
        }

        /// <summary>
        /// 格式化日志行
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="e">异常信息</param>
        /// <returns>格式化后的日志行</returns>
        private string FormatLogLine(LoggerLevel level, string message, Exception e)
        {
            message = $"{message}{Environment.NewLine}";
            if (e != null)
                message += $"{e.Message}{Environment.NewLine}{e.StackTrace}{Environment.NewLine}";

            return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} " +
                   $"[{Environment.CurrentManagedThreadId}] " +
                   $"{level.ToString().ToUpper()} {_type.FullName}{Environment.NewLine}" +
                   $"{message}{Environment.NewLine}";
        }

        private static StreamWriter GetWriter(string filePath)
        {
            return s_fileWriters.GetOrAdd(filePath, fp =>
                new StreamWriter(fp, append: true, encoding: Encoding.UTF8, bufferSize: 4096)
                {
                    AutoFlush = true   // 每条日志立即落盘，进程崩溃也不丢
                });
        }
    }
}