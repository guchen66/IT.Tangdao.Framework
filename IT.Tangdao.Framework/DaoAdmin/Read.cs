using IT.Tangdao.Framework.DaoAdmin;
using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.DaoDtos.Globals;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public sealed class Read : IRead
    {
        public string XMLData { get; set; }

        public string JsonData { get; set; }

        public string ReadObject
        {
            get => _readObject;
            set => _readObject = value;
        }

        private string _readObject;

        // 实现接口中的索引器
        public IRead this[string readObject]
        {
            get
            {
                _readObject = readObject;
                return this;
            }
        }

        public IReadResult SelectNode(string text)
        {
            var doc = XDocument.Parse(XMLData);
            var element = doc.Root.Element(text);

            if (element == null)
            {
                return new IReadResult($"Element '{text}' not found.", false);
            }

            string value = element.Value;
            return new IReadResult(value, true);
        }

        public IReadResult SelectNodes(string path)
        {
            XElement xElement = XElement.Load(path);
            List<XElement> xElements = xElement.Descendants().ToList();

            if (xElements == null)
            {
                return new IReadResult($"Element '{path}' not found.", false);
            }
            return new IReadResult(true, result: xElements);
        }

        public IReadResult<List<T>> SelectNodes<T>(string rootElement, Func<XElement, T> selector)
        {
            try
            {
                var doc = XDocument.Parse(XMLData);
                var elements = doc.Root.Elements().Select(node => node).ToList();

                if (elements == null || !elements.Any())
                {
                    return new IReadResult<List<T>>("未找到指定的元素。", false);
                }

                List<T> result = elements.Select(selector).ToList();
                return new IReadResult<List<T>>(true, result: result);
            }
            catch (Exception ex)
            {
                return new IReadResult<List<T>>($"解析 XML 失败: {ex.Message}", false);
            }
        }

        public IReadResult<List<T>> Descendants<T>(string rootElement, Func<XElement, T> selector) where T : class
        {
            try
            {
                var doc = XDocument.Parse(XMLData);

                // 使用 Descendants 查找所有匹配的元素
                var elements = doc.Descendants(rootElement).ToList();

                if (elements == null || !elements.Any())
                {
                    return new IReadResult<List<T>>("未找到指定的元素。", false);
                }

                // 使用 selector 函数映射每个元素到 T 类型的对象
                List<T> result = elements.Select(selector).ToList();
                return new IReadResult<List<T>>(true, result: result);
            }
            catch (Exception ex)
            {
                return new IReadResult<List<T>>($"解析 XML 失败: {ex.Message}", false);
            }
        }

        public IReadResult SelectKeys()
        {
            List<string> keys = new List<string>();
            JObject jsonObject = JObject.Parse(JsonData);

            // 递归方法，用于获取所有键
            void GetKeys(JToken token)
            {
                if (token is JObject obj)
                {
                    foreach (var property in obj.Properties())
                    {
                        keys.Add(property.Name);
                        GetKeys(property.Value);
                    }
                }
                else if (token is JArray array)
                {
                    foreach (var item in array)
                    {
                        GetKeys(item);
                    }
                }
            }

            GetKeys(jsonObject);
            return new IReadResult(true, keys);
        }

        public IReadResult SelectValue(string key)
        {
            var path = DirectoryHelper.SelectDirectoryByName(JsonData);
            string jsonContent = File.ReadAllText(path);
            JObject jsonObject = JObject.Parse(jsonContent);
            JToken valueToken = jsonObject[ReadObject][key];

            if (valueToken == null || valueToken.Type == JTokenType.Null)
            {
                // 键不存在或值为 null
                return new IReadResult("转换失败，JToken为null", false);
            }
            return new IReadResult(valueToken.ToString(), true);
        }

        /// <summary>
        /// 读取WPF自带的App.config
        /// 这两个引用没有传递值，是读取config的值，所以不需要使用ref，
        /// 使用了struct后，如果传递数据的扩展方法，需要加上ref
        /// </summary>
        /// <param name="menuList"></param>
        public IReadResult SelectConfig(string section)
        {
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);
            Dictionary<string, string> dict = idict.Cast<DictionaryEntry>().ToDictionary(de => de.Key.ToString(), de => de.Value.ToString());
            return new IReadResult(true, result: dict);
        }

        /// <summary>
        /// 读取自定义的config文件
        /// </summary>
        /// <param name="menuList"></param>
        public IReadResult SelectCustomConfig(string configName, string section)
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + configName)
            };

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            var customSection = (TangdaoMenuSection)config.GetSection(section);
            if (customSection == null)
            {
                dicts.Add("null", null);
                return new IReadResult(false, result: dicts);
            }
            foreach (MenuElement menu in customSection.Menus)
            {
                dicts.TryAdd(menu.Title, menu.Value);
            }
            return new IReadResult(true, result: dicts);
        }
    }
}