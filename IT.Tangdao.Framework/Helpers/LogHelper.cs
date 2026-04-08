using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Extensions;
using Newtonsoft.Json;
using IT.Tangdao.Framework.Enums;

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
                SaveAsXml(logItem, filePath);
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
        /// 以XML格式保存日志（追加模式）
        /// </summary>
        private static void SaveAsXml(LogItem logItem, string filePath)
        {
            if (!File.Exists(filePath))
            {
                // 文件不存在，创建包含单个日志项的列表
                var logItems = new List<LogItem> { logItem };
                TangdaoXmlSerializer.SerializeXMLToFile(logItems, filePath);
            }
            else
            {
                // 文件存在，需要追加
                try
                {
                    // 读取现有内容并反序列化为列表
                    string existingXml = File.ReadAllText(filePath);
                    var logItems = TangdaoXmlSerializer.Deserialize<List<LogItem>>(existingXml) ?? new List<LogItem>();

                    // 添加新的日志项
                    logItems.Add(logItem);

                    // 序列化回文件
                    TangdaoXmlSerializer.SerializeXMLToFile(logItems, filePath);
                }
                catch (Exception)
                {
                    // 如果出错，创建新的列表
                    var logItems = new List<LogItem> { logItem };
                    TangdaoXmlSerializer.SerializeXMLToFile(logItems, filePath);
                }
            }
        }

        /// <summary>
        /// 以JSON格式保存日志（追加模式）
        /// </summary>
        // <summary>
        /// 以JSON格式保存日志（追加模式）
        /// </summary>
        private static void SaveAsJson(LogItem logItem, string filePath)
        {
            if (!File.Exists(filePath))
            {
                // 文件不存在，创建包含单个日志项的数组
                var logItems = new List<LogItem> { logItem };
                TangdaoJsonFileHelper.SaveJsonData(logItems, filePath);
            }
            else
            {
                // 文件存在，读取现有数组并添加新项
                try
                {
                    string existingJson = File.ReadAllText(filePath);
                    var logItems = JsonConvert.DeserializeObject<List<LogItem>>(existingJson) ?? new List<LogItem>();
                    logItems.Add(logItem);
                    TangdaoJsonFileHelper.SaveJsonData(logItems, filePath);
                }
                catch (Exception)
                {
                    // 如果出错，创建新的数组
                    var logItems = new List<LogItem> { logItem };
                    TangdaoJsonFileHelper.SaveJsonData(logItems, filePath);
                }
            }
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