using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Paths;

namespace IT.Tangdao.Framework.Abstractions.Loggers
{
    /// <summary>
    /// 日志处理器
    /// </summary>
    internal class LoggerHandler : IDisposable
    {
        private readonly BlockingCollection<LogItem> _logQueue = new BlockingCollection<LogItem>();
        private readonly Task _processingTask;
        private readonly ConcurrentDictionary<string, StreamWriter> _fileWriters = new ConcurrentDictionary<string, StreamWriter>(StringComparer.Ordinal);
        private bool _isDisposed = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LoggerHandler()
        {
            _processingTask = Task.Run(async () => await ProcessLogQueueAsync());
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Dispose();
        }

        /// <summary>
        /// 将日志项加入队列
        /// </summary>
        /// <param name="logItem">日志项</param>
        public void EnqueueLog(LogItem logItem)
        {
            _logQueue.Add(logItem);
        }

        /// <summary>
        /// 处理日志队列
        /// </summary>
        private async Task ProcessLogQueueAsync()
        {
            try
            {
                foreach (var logItem in _logQueue.GetConsumingEnumerable())
                {
                    await ProcessLogItemAsync(logItem);
                }
            }
            catch (Exception ex)
            {
                // 避免日志处理线程崩溃
                Console.WriteLine($"日志处理线程异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理单个日志
        /// </summary>
        /// <param name="logItem"></param>
        /// <returns></returns>
        private async Task ProcessLogItemAsync(LogItem logItem)
        {
            // 格式化日志行
            string logLine = $"{logItem.Time:yyyy-MM-dd HH:mm:ss.fff} " +
                           $"[{logItem.ThreadId}] " +
                           $"{logItem.Level.ToString().ToUpper()} {logItem.Type.FullName}{Environment.NewLine}" +
                           $"{logItem.Message}{Environment.NewLine}";
            if (logItem.Exception != null)
                logLine += $"{logItem.Exception.Message}{Environment.NewLine}{logItem.Exception.StackTrace}{Environment.NewLine}";

            // 根据输出方式输出日志
            if (logItem.OutputType.HasFlag(LogOutputType.File))
            {
                var root = LogHelper.GetLogRoot();
                var extension = LogHelper.GetLogExtension();
                string filePath;

                // 检查是否使用日期路径
                var config = LogHelper.GetConfigList().LastOrDefault();
                if (config != null && config.UseDatePath)
                {
                    // 使用日期路径
                    var extensionWithoutDot = extension.TrimStart('.');
                    var fileName = $"{DateTime.Now.ToString("yyMMdd")}_Tangdao.{extensionWithoutDot}";
                    var datePath = TangdaoPath.Instance.DateFrom(root).BuildFile(fileName);
                    filePath = datePath.Value;
                }
                else
                {
                    // 使用普通路径
                    var fileName = $"Tangdao{extension}";
                    filePath = Path.Combine(root, fileName);
                }
                // 使用LogHelper保存日志
                LogHelper.SaveLogToFile(logItem, filePath);
            }

            if (logItem.OutputType.HasFlag(LogOutputType.Console))
            {
                // 根据日志级别设置控制台颜色
                ConsoleColor originalColor = Console.ForegroundColor;
                SetConsoleColor(logItem.Level);
                await Console.Out.WriteAsync(logLine);
                Console.ForegroundColor = originalColor;
            }

            if (logItem.OutputType.HasFlag(LogOutputType.Debug))
            {
                System.Diagnostics.Debug.Write(logLine);
            }
        }

        /// <summary>
        /// 设置控制台颜色
        /// </summary>
        /// <param name="level">日志级别</param>
        private static void SetConsoleColor(LoggerLevel level)
        {
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
        }

        /// <summary>
        /// 获取文件写入器
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件写入器</returns>
        private StreamWriter GetWriter(string filePath)
        {
            return _fileWriters.GetOrAdd(filePath, fp =>
            {
                // 确保目录存在
                var directory = Path.GetDirectoryName(fp);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                return new StreamWriter(fp, append: true, encoding: Encoding.UTF8, bufferSize: 4096)
                {
                    AutoFlush = true   // 每条日志立即落盘，进程崩溃也不丢
                };
            });
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                // 标记队列完成，不再接受新的日志
                _logQueue.CompleteAdding();
                // 等待日志处理完成
                try
                {
                    _processingTask.Wait(5000); // 最多等待5秒
                }
                catch (Exception)
                {
                    // 忽略超时异常
                }
                // 关闭所有文件写入器
                foreach (var writer in _fileWriters.Values)
                {
                    try
                    {
                        writer.Dispose();
                    }
                    catch (Exception)
                    {
                        // 忽略关闭异常
                    }
                }
                _fileWriters.Clear();
            }
        }
    }
}