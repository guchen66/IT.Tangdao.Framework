using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IT.Tangdao.Framework.Helpers
{
    public class XmlFolderHelper
    {
        /// <summary>
        /// 将对象转成XML，以字符串保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string SerializeXML<T>(T t)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(t.GetType());
                xmlSerializer.Serialize(sw, t);
                return sw.ToString();
            }
        }

        /// <summary>
        /// XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">xml字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml,bool nameSpace=false)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            if (nameSpace)
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty); // 禁用默认命名空间
            }
           
            using (StringReader stringReader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(stringReader);
            }
               
        }
    }
}
