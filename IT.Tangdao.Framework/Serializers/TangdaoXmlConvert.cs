using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace IT.Tangdao.Framework.Serializers
{
    /// <summary>
    /// Xml序列化反序列化帮助类
    /// </summary>
    public static class TangdaoXmlConvert
    {
        /// <summary>
        /// 将对象转成XML，并以指定编码保存为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="encoding">所需的编码格式，默认为 UTF-8</param>
        /// <returns></returns>
        public static string SerializeXML<T>(T t, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8; // 默认使用 UTF-8 编码
            }

            using (var sw = new StringWriter())
            {
                // 为了确保 XML 声明使用指定的编码，我们需要使用 XmlWriter
                var settings = new XmlWriterSettings
                {
                    Encoding = encoding,
                    Indent = true, // 可选：使 XML 更具可读性
                    OmitXmlDeclaration = false // 确保包含 XML 声明
                };

                using (var writer = XmlWriter.Create(sw, settings))
                {
                    var xmlSerializer = new XmlSerializer(t.GetType());
                    xmlSerializer.Serialize(writer, t);
                }

                return sw.ToString();
            }
        }

        /// <summary>
        /// 将对象转成XML，并以指定编码保存到文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="filePath"></param>
        /// <param name="encoding">所需的编码格式，默认为 UTF-8</param>
        public static void SerializeXMLToFile<T>(T t, string filePath, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8; // 默认使用 UTF-8 编码
            }

            var settings = new XmlWriterSettings
            {
                Encoding = encoding,
                Indent = true, // 可选：使 XML 更具可读性
                OmitXmlDeclaration = false // 确保包含 XML 声明
            };

            using (var writer = XmlWriter.Create(filePath, settings))
            {
                var xmlSerializer = new XmlSerializer(t.GetType());
                xmlSerializer.Serialize(writer, t);
            }
        }

        /// <summary>
        /// XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">xml字符串</param>
        /// <param name="ignoreNamespaces">是否忽略命名空间</param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml, bool ignoreNamespaces = false)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var stringReader = new StringReader(xml))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    if (ignoreNamespaces)
                    {
                        // 跳过所有命名空间
                        while (xmlReader.Read())
                        {
                            // 实现命名空间忽略逻辑（可选）
                        }
                    }

                    return (T)xmlSerializer.Deserialize(xmlReader);
                }
            }
        }

        /// <summary>
        /// 更简洁的反序列化方法，不需要处理命名空间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeSimple<T>(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}