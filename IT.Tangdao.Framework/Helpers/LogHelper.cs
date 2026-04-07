using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;
using Newtonsoft.Json;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    internal static class LogHelper
    {
        /// <summary>
        /// 获取日志根目录
        /// </summary>
        public static string GetLogRoot()
        {
            var config = TangdaoLoggerExtension.configs.LastOrDefault();
            if (config != null && !string.IsNullOrWhiteSpace(config.SaveDir))
            {
                var directory = config.SaveDir;
                _ = Directory.CreateDirectory(directory);
                return directory;
            }

            var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TangdaoLogs");
            _ = Directory.CreateDirectory(defaultPath);
            return defaultPath;
        }

        /// <summary>
        /// 获取所有日志配置
        /// </summary>
        public static List<LogEntry> GetConfigList()
        {
            return TangdaoLoggerExtension.configs;
        }

        /// <summary>
        /// 获取日志文件扩展名
        /// </summary>
        public static string GetLogExtension()
        {
            var config = TangdaoLoggerExtension.configs.LastOrDefault();
            var format = config?.LogFormat ?? LogFormat.Txt;

            switch (format)
            {
                case LogFormat.Xml:
                    return ".xml";

                case LogFormat.Json:
                    return ".json";

                case LogFormat.Txt:
                default:
                    return ".log";
            }
        }

        /// <summary>
        /// 保存日志到文件
        /// </summary>
        public static void SaveLogToFile(LogItem logItem, string filePath)
        {
            var extension = Path.GetExtension(filePath);

            if (extension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
            {
                // 保存为XML格式
                TangdaoXmlSerializer.SerializeXMLToFile(logItem, filePath);
            }
            else if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                // 保存为JSON格式
                SaveAsJson(logItem, filePath);
            }
            else
            {
                // 保存为文本格式
                SaveAsText(logItem, filePath);
            }
        }

        /// <summary>
        /// 以JSON格式保存日志
        /// </summary>
        private static void SaveAsJson(LogItem logItem, string filePath)
        {
            string json = JsonConvert.SerializeObject(logItem, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// 以文本格式保存日志
        /// </summary>
        private static void SaveAsText(LogItem logItem, string filePath)
        {
            string logLine = FormatLogLine(logItem);
            File.AppendAllText(filePath, logLine);
        }

        /// <summary>
        /// 格式化日志行
        /// </summary>
        private static string FormatLogLine(LogItem logItem)
        {
            string message = $"{logItem.Message}{Environment.NewLine}";
            if (!string.IsNullOrEmpty(logItem.ExceptionMessage))
                message += $"{logItem.ExceptionMessage}{Environment.NewLine}{logItem.ExceptionStackTrace}{Environment.NewLine}";

            return $"{logItem.Time:yyyy-MM-dd HH:mm:ss.fff} " +
                   $"[{logItem.ThreadId}] " +
                   $"{logItem.Level.ToString().ToUpper()} {logItem.TypeName}{Environment.NewLine}" +
                   $"{message}{Environment.NewLine}";
        }
    }
}