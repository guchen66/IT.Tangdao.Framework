using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Templates;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Configurations
{
    /// <summary>
    /// 日志配置确保类
    /// </summary>
    public static class LogEnsureConfig
    {
        private static string _savePath;
        private static TangdaoLogInterval _logInterval;
        private static TangdaoLogTemplate _logTemplate;
        private static readonly object _lock = new object();

        /// <summary>
        /// 加载日志配置
        /// </summary>
        /// <param name="logEntry">日志配置项</param>
        public static void Load(LogEntry logEntry)
        {
            if (logEntry == null)
                throw new ArgumentNullException(nameof(logEntry));

            lock (_lock)
            {
                if (!string.IsNullOrWhiteSpace(logEntry.SaveDir))
                {
                    _ = Directory.CreateDirectory(logEntry.SaveDir);
                    _savePath = logEntry.SaveDir;
                }

                _logInterval = logEntry.LogInterval;
                _logTemplate = logEntry.Template;
            }
        }

        /// <summary>
        /// 获取日志根目录
        /// </summary>
        public static string Root
        {
            get
            {
                if (_savePath == null)
                {
                    lock (_lock)
                    {
                        if (_savePath == null)
                            _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TangdaoLogs");
                        _ = Directory.CreateDirectory(_savePath);
                    }
                }
                return _savePath;
            }
        }

        /// <summary>
        /// 获取日志间隔
        /// </summary>
        public static TangdaoLogInterval LogInterval
        {
            get { return _logInterval; }
        }

        /// <summary>
        /// 获取日志模板
        /// </summary>
        public static TangdaoLogTemplate LogTemplate
        {
            get { return _logTemplate; }
        }
    }
}