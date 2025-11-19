using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Configurations;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// INI 文件解析器
    /// </summary>
    internal static class IniParser
    {
        /// <summary>
        /// 解析 INI 文件内容
        /// </summary>
        public static IniConfigCollection Parse(string iniContent)
        {
            // 使用 static readonly 提高性能
            var configs = new IniConfigCollection();
            IniConfig currentSection = null;

            // 使用预定义的字符数组，避免每次调用都创建新数组
            foreach (var line in iniContent.Split(Separators.Line, StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmedLine = line.Trim();

                // 跳过空行和注释
                if (string.IsNullOrEmpty(trimmedLine) ||
                    trimmedLine.StartsWith(";") ||
                    trimmedLine.StartsWith("#"))
                    continue;

                // 处理节标题 [SectionName]
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = new IniConfig
                    {
                        Section = trimmedLine.Substring(1, trimmedLine.Length - 2).Trim()
                    };
                    configs.Add(currentSection);
                    continue;
                }

                // 处理键值对 key=value
                var equalsIndex = trimmedLine.IndexOf('=');
                if (equalsIndex > 0 && currentSection != null)
                {
                    var key = trimmedLine.Substring(0, equalsIndex).Trim();
                    var value = trimmedLine.Substring(equalsIndex + 1).Trim();
                    currentSection.KeyValues[key] = value;
                }
            }

            return configs;
        }

        /// <summary>
        /// 从文件路径解析 INI 文件
        /// </summary>
        public static IniConfigCollection ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"INI file not found: {filePath}");

            var content = File.ReadAllText(filePath);
            return Parse(content);
        }

        /// <summary>
        /// 检测内容是否为 INI 文件格式
        /// </summary>
        public static bool IsIniFormat(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            var lines = content.Split(Separators.Line, StringSplitOptions.RemoveEmptyEntries);

            // 统计 INI 格式特征
            int sectionCount = 0;
            int keyValueCount = 0;
            int totalLines = 0;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // 跳过空行和注释
                if (string.IsNullOrEmpty(trimmedLine) ||
                    trimmedLine.StartsWith(";") ||
                    trimmedLine.StartsWith("#"))
                    continue;

                totalLines++;

                // 检测节标题 [SectionName]
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    sectionCount++;
                }
                // 检测键值对 key=value
                else if (ContainsKeyValuePair(trimmedLine))
                {
                    keyValueCount++;
                }
            }

            // 如果包含节标题或者有键值对格式，认为是 INI 文件
            return (sectionCount > 0 && keyValueCount > 0) ||
                   (keyValueCount > 0 && keyValueCount >= totalLines * 0.7);
        }

        /// <summary>
        /// 检测行是否为键值对格式
        /// </summary>
        private static bool ContainsKeyValuePair(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;

            var equalsIndex = line.IndexOf('=');
            if (equalsIndex <= 0 || equalsIndex >= line.Length - 1)
                return false;

            var keyPart = line.Substring(0, equalsIndex).Trim();
            var valuePart = line.Substring(equalsIndex + 1).Trim();

            // 基本的键值对验证
            return !string.IsNullOrEmpty(keyPart) &&
                   !keyPart.Contains(" ") &&
                   valuePart.Length > 0;
        }
    }
}