using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public class DaoLogger : IDaoLogger
    {
        private static readonly ConcurrentDictionary<Type, IDaoLogger> Loggers = new ConcurrentDictionary<Type, IDaoLogger>();

        public static Func<Type, IDaoLogger> LoggerFactory { get; set; }

        public static IDaoLogger Get(Type type)
        {
            //TryGetValue避免两次查找，直接返回logger
            if (!Loggers.TryGetValue(type, out IDaoLogger logger))
            {
                logger = LoggerFactory?.Invoke(type) ?? new DaoLogger(type);
                Loggers[type] = logger; // 或者使用 TryAdd 更安全
            }

            return logger;
        }

        private readonly Type _type;

        protected DaoLogger(Type type) => _type = type;

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
            {
                message = $"{message}{e.Message}{Environment.NewLine}" +
                          $"{e.StackTrace}{Environment.NewLine}";
            }

            message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} " +
                      $"[{Thread.CurrentThread.ManagedThreadId}] " +
                      $"{category.ToUpper()} " +
                      $"{_type.FullName}{Environment.NewLine}" +
                      $"{message}{Environment.NewLine}";

            System.Diagnostics.Debug.Write(message);
            File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Tangdao.log"), message);
        }
    }
}