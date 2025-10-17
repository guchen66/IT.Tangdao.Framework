using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 只给 XDocument / XElement 做扩展
    /// </summary>
    public static class XmlExtension
    {
        /// <summary>
        /// 从文件路径安全加载 XML 文档
        /// </summary>
        public static XDocument LoadFromFile(this string filePath)
        {
            return XDocument.Load(filePath);
        }

        /// <summary>
        /// 任意 XNode 是否包含有效元素（用来快速判断“空 XML”）
        /// </summary>
        public static bool IsEmpty(this XElement root)
            => !root.Descendants().Any();

        /// <summary>
        /// XDocument 直接拿 Root，省得每次都 .Root
        /// </summary>
        public static XElement RootElement(this XDocument doc)
            => doc.Root;

        /// <summary>
        /// 安全拿元素值；元素不存在返回 default
        /// </summary>
        public static string ValueOrDefault(this XElement e, string defaultVal = "")
            => e?.Value ?? defaultVal;

        /// <summary>
        /// 一路点下去，任意一级 null 就返回 null，不抛。
        /// </summary>
        public static XElement Descend(this XElement parent, params string[] tags)
            => tags.Aggregate(parent, (current, tag) => current.Element(tag));

        /// <summary>
        /// 同级全部子节点。
        /// </summary>
        public static IEnumerable<XElement> Children(this XElement parent, string tag)
            => parent.Elements(tag);

        /// <summary>
        /// 同级过滤属性。
        /// </summary>
        public static IEnumerable<XElement> WhereAttr(this IEnumerable<XElement> source,
                                                      string attrName, string attrValue)
            => source.Where(e => (string)e.Attribute(attrName) == attrValue);

        /// <summary>
        /// 把一组节点直接映射成对象，LINQ 一行完事。
        /// </summary>
        public static IEnumerable<T> Map<T>(this IEnumerable<XElement> source,
                                            Func<XElement, T> projector)
            => source.Select(projector);

        #region XElement 扩展 - 安全访问

        /// <summary>
        /// 安全获取子元素，如果不存在则返回 null
        /// </summary>
        public static XElement GetElement(this XElement element, XName name)
        {
            return element?.Element(name);
        }

        /// <summary>
        /// 安全获取元素值，如果不存在则返回默认值
        /// </summary>
        public static string GetElementValue(this XElement element, XName name, string defaultValue = null)
        {
            return element?.Element(name)?.Value ?? defaultValue;
        }

        /// <summary>
        /// 安全获取元素值并转换为指定类型
        /// </summary>
        public static T GetElementValue<T>(this XElement element, XName name, T defaultValue = default)
        {
            var value = element?.Element(name)?.Value;
            return value != null ? (T)Convert.ChangeType(value, typeof(T)) : defaultValue;
        }

        /// <summary>
        /// 安全获取属性值
        /// </summary>
        public static string GetAttributeValue(this XElement element, XName name, string defaultValue = null)
        {
            return element?.Attribute(name)?.Value ?? defaultValue;
        }

        #endregion XElement 扩展 - 安全访问

        #region LINQ 风格的 XML 查询扩展

        /// <summary>
        /// 对 XML 元素进行 LINQ 查询
        /// </summary>
        public static IEnumerable<XElement> WhereElement(this XElement element, Func<XElement, bool> predicate)
        {
            return element.Elements().Where(predicate);
        }

        /// <summary>
        /// 按元素名称筛选
        /// </summary>
        public static IEnumerable<XElement> WhereElementName(this XElement element, XName name)
        {
            return element.Elements().Where(e => e.Name == name);
        }

        /// <summary>
        /// 按属性值筛选
        /// </summary>
        public static IEnumerable<XElement> WhereAttribute(this XElement element, XName attributeName, string attributeValue)
        {
            return element.Elements().Where(e => e.Attribute(attributeName)?.Value == attributeValue);
        }

        /// <summary>
        /// 选择元素值并转换为指定类型
        /// </summary>
        public static IEnumerable<T> SelectElementValues<T>(this XElement element, XName elementName)
        {
            return element.Elements(elementName).Select(e => (T)Convert.ChangeType(e.Value, typeof(T)));
        }

        #endregion LINQ 风格的 XML 查询扩展

        #region 转换和序列化扩展

        /// <summary>
        /// 将对象转换为 XElement（简化版）
        /// </summary>
        public static XElement ToXElement<T>(this T obj, string rootName = "Item") where T : class, IXmlSerializable
        {
            var element = new XElement(rootName);
            foreach (var prop in typeof(T).GetProperties())
            {
                element.Add(new XElement(prop.Name, prop.GetValue(obj)?.ToString()));
            }
            return element;
        }

        /// <summary>
        /// 将 XElement 转换为对象
        /// </summary>
        public static T ToObject<T>(this XElement element) where T : class, new()
        {
            var obj = new T();
            foreach (var prop in typeof(T).GetProperties())
            {
                var value = element.GetElementValue(prop.Name);
                if (value != null)
                {
                    prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                }
            }
            return obj;
        }

        /// <summary>
        /// 将元素集合转换为对象列表
        /// </summary>
        public static List<T> ToObjectList<T>(this IEnumerable<XElement> elements) where T : class, new()
        {
            return elements.Select(e => e.ToObject<T>()).ToList();
        }

        #endregion 转换和序列化扩展

        #region 验证和检查扩展

        /// <summary>
        /// 检查元素是否存在
        /// </summary>
        public static bool HasElement(this XElement element, XName name)
        {
            return element?.Element(name) != null;
        }

        /// <summary>
        /// 检查属性是否存在
        /// </summary>
        public static bool HasAttribute(this XElement element, XName name)
        {
            return element?.Attribute(name) != null;
        }

        /// <summary>
        /// 验证 XML 是否符合某个条件
        /// </summary>
        public static bool Validate(this XElement element, Func<XElement, bool> validator)
        {
            return validator(element);
        }

        #endregion 验证和检查扩展

        #region 批量操作扩展

        /// <summary>
        /// 批量添加子元素
        /// </summary>
        public static void AddElements(this XElement parent, params XElement[] elements)
        {
            parent.Add(elements);
        }

        /// <summary>
        /// 批量设置属性
        /// </summary>
        public static void SetAttributes(this XElement element, params (XName Name, string Value)[] attributes)
        {
            foreach (var (name, value) in attributes)
            {
                element.SetAttributeValue(name, value);
            }
        }

        /// <summary>
        /// 递归获取所有后代元素（扁平化）
        /// </summary>
        public static IEnumerable<XElement> DescendantsFlattened(this XElement element)
        {
            return element.Descendants();
        }

        #endregion 批量操作扩展
    }
}