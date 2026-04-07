using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 日志扩展类
    /// </summary>
    public static class TangdaoLoggerExtension
    {
        /// <summary>
        /// 把日志写到应用层指定的目录（或默认桌面）。
        /// category 为空时直接落在根目录，否则作为子文件夹。
        /// 程序启动LogPathConfig.SetRoot可自定义Log目录
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> _fileLocks = new ConcurrentDictionary<string, object>();

        internal static List<LogEntry> configs = new List<LogEntry>();

        public static void WriteLocal(this ITangdaoLogger logger, string message, string category = null, [CallerMemberName] string caller = null,
            [CallerFilePath] string file = null,
            [CallerLineNumber] int line = 0)
        {
            if (logger == null) return;

            try
            {
                var root = LogHelper.GetLogRoot();
                var dir = string.IsNullOrEmpty(category) ? root : Path.Combine(root, category);
                Directory.CreateDirectory(dir);

                var extension = LogHelper.GetLogExtension();
                var fileName = $"{DateTime.Now:yyyyMMdd}{extension}";
                var filePath = Path.Combine(dir, fileName);

                // 创建日志项
                var logItem = new LogItem
                {
                    Time = DateTime.Now,
                    ThreadId = Environment.CurrentManagedThreadId,
                    Level = IT.Tangdao.Framework.Enums.LoggerLevel.Info,
                    Type = logger.GetType(),
                    Caller = caller,
                    File = Path.GetFileName(file),
                    Line = line,
                    Message = message
                };

                // 获取或创建针对该文件路径的锁
                var fileLock = _fileLocks.GetOrAdd(filePath, path => new object());

                lock (fileLock)
                {
                    // 使用LogHelper保存日志
                    LogHelper.SaveLogToFile(logItem, filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WriteLocal failed: {ex}");
            }
        }
    }
}