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

        public static void WriteLocal(this ITangdaoLogger logger, string message, string category = null, [CallerMemberName] string caller = null,
            [CallerFilePath] string file = null,
            [CallerLineNumber] int line = 0)
        {
            if (logger == null) return;

            try
            {
                var root = LogPathConfig.Root;
                var dir = string.IsNullOrEmpty(category) ? root : Path.Combine(root, category);
                Directory.CreateDirectory(dir);

                var fileName = $"{DateTime.Now:yyyyMMdd}.log";
                var filePath = Path.Combine(dir, fileName);

                var logLine = $"ThreadId:{Environment.CurrentManagedThreadId} {DateTime.Now:HH:mm:ss.fff}  [{caller}]  ({Path.GetFileName(file)}:{line})  " +
                             $"{logger.GetType().FullName}  {message}{Environment.NewLine}";

                // 获取或创建针对该文件路径的锁
                var fileLock = _fileLocks.GetOrAdd(filePath, path => new object());

                lock (fileLock)
                {
                    File.AppendAllText(filePath, logLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WriteLocal failed: {ex}");
            }
        }
    }
}