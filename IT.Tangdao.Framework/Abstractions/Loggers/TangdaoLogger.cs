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

        protected TangdaoLogger(Type type) => _type = type;

        public void Fatal(string message, Exception e = null)
        {
            Write(nameof(Fatal), message, e);
        }

        public void Error(string message, Exception e = null)
        {
            Write(nameof(Error), message, e);
        }

        public void Warning(string message, Exception e = null)
        {
            Write(nameof(Warning), message, e);
        }

        public void Info(string message, Exception e = null)
        {
            Write(nameof(Info), message, e);
        }

        public void Debug(string message, Exception e = null)
        {
            Write(nameof(Debug), message, e);
        }

        private void Write(string category, string message, Exception e)
        {
            message = $"{message}{Environment.NewLine}";
            if (e != null)
                message += $"{e.Message}{Environment.NewLine}{e.StackTrace}{Environment.NewLine}";

            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} " +
                      $"[{Environment.CurrentManagedThreadId}] " +
                      $"{category.ToUpper()} {_type.FullName}{Environment.NewLine}" +
                      $"{message}{Environment.NewLine}";

            var file = System.IO.Path.Combine(LogPathConfig.Root, "Tangdao.log");
            var writer = GetWriter(file);
            writer.Write(line);          // 无锁，ConcurrentDictionary 已保证
            System.Diagnostics.Debug.Write(line);
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