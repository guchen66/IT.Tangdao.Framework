using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IT.Tangdao.Framework.Helpers
{
    public class TangdaoXmlSerializer
    {
        /// <summary>
        /// 将对象序列化为 XML 字符串，并指定编码格式（默认 UTF-8）
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="encoding">指定的编码格式，默认为 UTF-8</param>
        /// <returns>XML 字符串</returns>
        public static string SerializeXML<T>(T obj, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8; // 默认使用 UTF-8 编码
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = encoding,
                Indent = true, // 可选：使 XML 更具可读性
                OmitXmlDeclaration = false, // 确保包含 XML 声明
            };

            using (StringWriterWithEncoding stringWriter = new StringWriterWithEncoding(encoding))
            using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, obj);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// 将对象序列化为 XML 文件，并指定编码格式（默认 UTF-8）
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="filePath">保存的文件路径</param>
        /// <param name="encoding">指定的编码格式，默认为 UTF-8</param>
        public static void SerializeXMLToFile<T>(T obj, string filePath, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8; // 默认使用 UTF-8 编码
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = encoding,
                Indent = true, // 可选：使 XML 更具可读性
                OmitXmlDeclaration = false // 确保包含 XML 声明
            };

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// 将 XML 字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">XML 字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static T Deserialize<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// 自定义 StringWriter，支持指定编码
        /// </summary>
        private class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding _encoding;

            public StringWriterWithEncoding(Encoding encoding)
            {
                _encoding = encoding;
            }

            public override Encoding Encoding => _encoding;
        }
    }
}