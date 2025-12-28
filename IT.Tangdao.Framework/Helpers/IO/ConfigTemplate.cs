using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace IT.Tangdao.Framework.Helpers.IO
{
    /// <summary>
    /// 配置文件模板类，用于检测和处理配置文件内容
    /// 专注于配置文件特定逻辑，与XML检测分离
    /// </summary>
    internal class ConfigTemplate
    {
        /// <summary>
        /// 用于检测配置文件外形的正则表达式
        /// 识别常见的.NET配置文件结构
        /// </summary>
        private static readonly Regex ConfigLikePattern = new Regex(
            @"^\s*(<\?xml.*?\?>)?\s*<(Configuration|appSettings|connectionStrings|system\.web|system\.serviceModel)>",
            RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 配置文件根元素名称集合
        /// </summary>
        public static readonly string[] ConfigRootElements =
        {
            "Configuration",
            "appSettings",
            "connectionStrings",
            "system.web",
            "system.serviceModel"
        };

        /// <summary>
        /// 极速过滤：判断文本是否可能是配置文件格式
        /// 快速排除非配置文件内容
        /// </summary>
        /// <param name="content">要检测的文本内容</param>
        /// <returns>是否可能是配置文件格式</returns>
        public static bool MaybeConfig(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            return ConfigLikePattern.IsMatch(content);
        }

        /// <summary>
        /// 结构验证：使用XmlReader检测配置文件是否格式良好
        /// 先验证是否为有效的XML，再验证是否包含配置文件特定元素
        /// </summary>
        /// <param name="content">要检测的文本内容</param>
        /// <returns>是否是格式良好的配置文件</returns>
        public static bool IsWellFormedConfig(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            // 首先验证是否为有效的XML
            if (!XmlTemplate.IsValidXml(content))
                return false;

            try
            {
                using (var strReader = new StringReader(content))
                {
                    using (var xmlReader = XmlReader.Create(strReader, new XmlReaderSettings
                    {
                        XmlResolver = null,
                        ConformanceLevel = ConformanceLevel.Document,
                        DtdProcessing = DtdProcessing.Prohibit,
                        ValidationType = ValidationType.None
                    }))
                    {
                        // 查找配置文件特有的根元素
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                string localName = xmlReader.LocalName;
                                string namespaceUri = xmlReader.NamespaceURI;
                                string fullName = string.IsNullOrEmpty(namespaceUri)
                                    ? localName
                                    : $"{namespaceUri}:{localName}";

                                // 检查是否包含配置文件特有的根元素
                                foreach (string configElement in ConfigRootElements)
                                {
                                    if (string.Equals(fullName, configElement, StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(localName, configElement, StringComparison.OrdinalIgnoreCase))
                                    {
                                        return true;
                                    }
                                }

                                // 检查是否包含配置相关的子元素
                                if (xmlReader.ReadToDescendant("appSettings") ||
                                    xmlReader.ReadToDescendant("connectionStrings") ||
                                    xmlReader.ReadToDescendant("add", "") ||
                                    xmlReader.ReadToDescendant("setting", ""))
                                {
                                    return true;
                                }

                                break;
                            }
                        }

                        return false;
                    }
                }
            }
            catch (XmlException)
            {
                return false;
            }
        }

        /// <summary>
        /// 综合检测：结合极速过滤和结构验证
        /// 先轻量判断，再heavyweight验证，提高性能
        /// </summary>
        /// <param name="content">要检测的文本内容</param>
        /// <returns>是否是有效的配置文件</returns>
        public static bool IsValidConfig(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            // 第一步：极速过滤，排除明显不是配置文件的内容
            if (!MaybeConfig(content))
                return false;

            // 第二步：结构验证，确保配置文件格式良好且包含配置特定元素
            return IsWellFormedConfig(content);
        }
    }
}