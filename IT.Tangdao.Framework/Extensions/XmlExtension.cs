using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IT.Tangdao.Framework.Extensions
{
    public static class XmlExtension
    {
        public static XDocument ToXmlStream(this string path)
        {
            return XDocument.Load(path);
        }

        public static XElement ReadXmlElementRoot(this XDocument element) 
        {
             return element.Root;
        }

        public static XElement ReadXmlElementRoot(this XDocument element,string rootStr)
        {
            return element.Root.Element(rootStr);
        }

        public static string ToXmlString(this XElement element,string name) 
        {
            return element.Element(name).Value;
        }
    }
}
