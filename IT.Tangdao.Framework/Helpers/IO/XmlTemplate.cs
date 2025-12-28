using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// XML模板类，用于检测和处理XML内容
    /// </summary>
    internal class XmlTemplate
    {
        /// <summary>
        /// 用于检测XML外形的正则表达式
        /// 最简：一个根元素，支持XML声明
        /// </summary>
        private static readonly Regex XmlLikePattern = new Regex(
            @"^\s*(<\?xml.*?\?>)?\s*<[^>]+>(.*</[^>]+>\s*)$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// 极速过滤：判断文本是否可能是XML格式
        /// 10万段文本1ms级，能把JSON等直接排除
        /// 不保证语义正确，仅做初步过滤
        /// </summary>
        /// <param name="content">要检测的文本内容</param>
        /// <returns>是否可能是XML格式</returns>
        public static bool MaybeXml(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            return XmlLikePattern.IsMatch(content);
        }

        /// <summary>
        /// 结构验证：使用XmlReader检测XML是否格式良好
        /// 比XDocument.Parse快3~5倍，不实例化DOM，内存恒定
        /// </summary>
        /// <param name="content">要检测的文本内容</param>
        /// <param name="conformanceLevel">XML一致性级别，默认为Document</param>
        /// <returns>是否是格式良好的XML</returns>
        public static bool IsWellFormedXml(string content, ConformanceLevel conformanceLevel = ConformanceLevel.Document)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            try
            {
                using (var strReader = new StringReader(content))
                {
                    using (var xmlReader = XmlReader.Create(strReader, new XmlReaderSettings
                    {
                        XmlResolver = null,
                        ConformanceLevel = conformanceLevel,
                        DtdProcessing = DtdProcessing.Prohibit,
                        ValidationType = ValidationType.None
                    }))
                    {
                        // 只向前扫一遍，能走到末尾=良构
                        while (xmlReader.Read()) { }
                        return true;
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
        /// <returns>是否是有效的XML格式</returns>
        public static bool IsValidXml(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            // 第一步：极速过滤，排除明显不是XML的内容
            if (!MaybeXml(content))
                return false;

            // 第二步：结构验证，确保XML格式良好
            return IsWellFormedXml(content);
        }
    }
}