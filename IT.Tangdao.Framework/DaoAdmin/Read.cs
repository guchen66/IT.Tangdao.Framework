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

        private string _configData;

        public string ConfigData
        {
            get => _configData;
            set
            {
                _configData = value;
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

                case DaoFileType.Config:
                    ConfigData = data;
                    break;

                default:
                    throw new InvalidOperationException($"不支持的文件类型: {_fileType}");
            }
        }

        #region-- 读取XML数据 --

        public IReadResult SelectNode(string node)
        {
            if (XMLData == null)
            {
                return new IReadResult("未进行Load操作", false);
            }

            try
            {
                var doc = XDocument.Parse(XMLData);
                var root = doc.Root;
                var xmlType = FileSelector.DetectXmlStructure(doc);

                switch (xmlType)
                {
                    case DaoXmlType.Empty:
                        return new IReadResult("XML内容为空", false);

                    case DaoXmlType.None:
                        return new IReadResult("XML只有声明没有内容", false);

                    case DaoXmlType.Single:
                        // 单节点结构 - 直接查找目标节点
                        var singleElement = root.Element(node) ?? root.Elements().First().Element(node);
                        if (singleElement == null)
                        {
                            return new IReadResult($"节点 '{node}' 不存在", false);
                        }
                        return new IReadResult(singleElement.Value, true);

                    case DaoXmlType.Multiple:
                        if (ReadIndex < 0)
                        {
                            // 尝试直接查找节点(适用于扁平结构)
                            var directElement = root.Element(node);
                            if (directElement != null)
                            {
                                return new IReadResult(directElement.Value, true);
                            }
                            return new IReadResult("存在多个节点，请指定索引", false);
                        }
                        else
                        {
                            // 处理指定索引的情况
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

                    default:
                        return new IReadResult("未知的XML结构类型", false);
                }
            }
            catch (Exception ex)
            {
                return new IReadResult($"XML解析错误: {ex.Message}", false);
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

        #endregion

        #region-- 读取Json数据 --

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

        #endregion

        #region-- 读取Config数据--

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

        public IReadResult SelectConfig<T>(string section) where T : class, new()
        {
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);

            var dict = idict.Cast<DictionaryEntry>()
                           .ToDictionary(
                               de => de.Key.ToString(),
                               de => Convert.ChangeType(de.Value, typeof(T)) as T
                           );

            return new IReadResult(true, result: dict);
        }

        public IReadResult SelectConfigByJsonConvert<T>(string section) where T : class, new()
        {
            T Target = new T();
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);
            var dict = idict.Cast<DictionaryEntry>()
                   .ToDictionary(
                       de => de.Key.ToString(),
                       de => JsonConvert.DeserializeObject<T>(de.Value.ToString())
                   );

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

        #endregion
    }
}