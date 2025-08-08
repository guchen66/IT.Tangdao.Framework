using IT.Tangdao.Framework.DaoAdmin;
using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.DaoDtos.Globals;
using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.DaoSelectors;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public sealed class Read : IRead
    {
        // 属性改造（自动同步_fileType）
        private string _xmlData;

        public string XMLData
        {
            get => _xmlData;
            set
            {
                _xmlData = value;
            }
        }

        private string _jsonData;

        public string JsonData
        {
            get => _jsonData;
            set
            {
                _jsonData = value;
            }
        }

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

        public int ReadIndex
        {
            get => _readIndex;
            set => _readIndex = value;
        }

        private int _readIndex = -1;

        // 实现接口中的索引器
        public IRead this[int readIndex]
        {
            get
            {
                _readIndex = readIndex;
                return this;
            }
        }

        private DaoFileType _fileType;

        public void Load(string data)
        {
            _fileType = FileSelector.DetectFromContent(data);
            switch (_fileType)
            {
                case DaoFileType.Xml:
                    XMLData = data;
                    break;

                case DaoFileType.Json:
                    JsonData = data;
                    break;

                default:
                    throw new InvalidOperationException($"不支持的文件类型: {_fileType}");
            }
        }

        public IReadResult SelectNode(string node)
        {
            if (XMLData == null)
            {
                return new IReadResult("未进行Load操作", false);
            }

            var doc = XDocument.Parse(XMLData);
            var root = doc.Root;

            // 情况1：没有指定索引（Current.SelectNode）
            if (ReadIndex < 0) // 假设未设置索引时 ReadIndex = -1
            {
                // 如果是单节点结构，直接读取
                if (root.Elements().Count() == 1)
                {
                    var element = root.Element(node);
                    if (element == null)
                    {
                        return new IReadResult($"Element '{node}' not found.", false);
                    }
                    return new IReadResult(element.Value, true);
                }
                // 否则返回错误（因为不知道要读哪个父节点）
                return new IReadResult("存在多个节点，请指定索引", false);
            }
            // 情况2：指定了索引（Current[1].SelectNode）
            else
            {
                var parentNode = root.Elements().ElementAtOrDefault(ReadIndex);
                if (parentNode == null)
                {
                    return new IReadResult($"索引 {ReadIndex} 超出范围", false);
                }

                var targetElement = parentNode.Element(node);
                if (targetElement == null)
                {
                    return new IReadResult($"子节点 '{node}' 不存在", false);
                }

                return new IReadResult(targetElement.Value, true);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="path">这里的path是uri地址，不是XML具体数据</param>
        /// <returns></returns>
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
                if (XMLData == null)
                {
                    return new IReadResult<List<T>>($"未进行Load操作", false);
                }
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

        public IReadResult SelectKeys()
        {
            if (JsonData == null)
            {
                return new IReadResult($"未进行Load操作", false);
            }
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

        /// <summary>
        /// 跟据key读取指定value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IReadResult SelectValue(string key)
        {
            if (JsonData == null)
            {
                return new IReadResult($"未进行Load操作", false);
            }
            var path = DirectoryHelper.SelectDirectoryByName(JsonData);
            string jsonContent = File.ReadAllText(path);
            JObject jsonObject = JObject.Parse(jsonContent);
            if (ReadObject == null)
            {
                return new IReadResult("转换失败，未设置索引器", false);
            }
            JToken valueToken = jsonObject.SelectToken($"{ReadObject}.{key}");

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

        public IReadResult<List<T>> SelectNodes<T>() where T : new()
        {
            try
            {
                if (XMLData == null)
                    return new IReadResult<List<T>>("未进行Load操作", false);

                var doc = XDocument.Parse(XMLData);
                var result = new List<T>();

                foreach (var node in doc.Root.Elements())
                {
                    var instance = new T();
                    FileSelector.MapXElementToObject(node, instance); // 自动映射
                    result.Add(instance);
                }

                return new IReadResult<List<T>>(true, result);
            }
            catch (Exception ex)
            {
                return new IReadResult<List<T>>($"解析失败: {ex.Message}", false);
            }
        }
    }
}