using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Paths;
using System.Xml;
using IT.Tangdao.Framework.Configurations;
using System.Reflection;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    internal sealed class ContentQueryable : IContentQueryable, IXmlQueryable, IJsonQueryable, IConfigQueryable, IIniQueryable
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(ContentQueryable));
        private string _content = string.Empty;

        public string Content
        {
            get => _content;
            set => _content = value;
        }

        public string ReadPath { get; set; }

        public DaoFileType DetectedType { get; set; }

        /* ========== IContentQueryable 通用方法 **只写一次** ========== */

        public IContentQueryable Auto() => this;   // 逻辑已在内置字段，直接返回自己

        /* ========== 格式切换 ========== */

        public IXmlQueryable AsXml()
        {
            if (DetectedType == DaoFileType.Xml)
            {
                return this;
            }
            throw new FormatException("转换失败，读取的内容不是有效的XML格式");
        }

        public IJsonQueryable AsJson()
        {
            if (DetectedType == DaoFileType.Json)
            {
                return this;
            }
            throw new FormatException("转换失败，读取的内容不是有效的Json格式");
        }

        public IConfigQueryable AsConfig()
        {
            // 如果是 Config 类型，或者是未读取状态（用于读取默认 App.config），都允许转换
            if (DetectedType == DaoFileType.Config || DetectedType == DaoFileType.None)
            {
                return this;
            }
            throw new FormatException("转换失败，读取的内容不是有效的Config格式");
        }

        public IIniQueryable AsIni()
        {
            if (DetectedType == DaoFileType.Ini)
            {
                return this;
            }
            throw new FormatException("转换失败，读取的内容不是有效的Ini格式");
        }

        #region--XML读取--

        /// <summary>
        /// XML结构是单个节点使用
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ResponseResult SelectNode(string node)
        {
            if (_content == null)
            {
                return ResponseResult.Failure("文件未解析成功", new XmlException($"文件未解析成功 {_content}"));
            }

            try
            {
                var doc = XDocument.Parse(_content);
                var root = doc.RootElement();
                var xmlType = FileHelper.DetectXmlStructure(doc);

                switch (xmlType)
                {
                    case XmlStruct.Empty:
                        return ResponseResult.Failure("XML内容为空", new XmlException($"XML内容为空 {node}"));

                    case XmlStruct.None:
                        return ResponseResult.Failure("XML只有声明没有内容", new XmlException($"XML只有声明没有内容: {node}"));

                    case XmlStruct.Single:
                        // 单节点结构 - 直接查找目标节点
                        var singleElement = root.Element(node) ?? root.Elements().First().Element(node);
                        if (singleElement == null)
                        {
                            return ResponseResult.Failure("存在多个节点，请指定索引", new XmlException($"存在多个节点，请指定索引，目标节点: {node}"));
                        }
                        return ResponseResult.Success(value: singleElement.Value);

                    case XmlStruct.Multiple:
                        var directElement = root.Element(node);
                        if (directElement != null)
                        {
                            return ResponseResult.Success(value: directElement.Value);
                        }
                        return ResponseResult.Failure("存在多个节点，请使用SelectNodes方法", new XmlException($"存在多个节点，请指定索引，目标节点: {node}"));

                    default:
                        return ResponseResult.Failure("未知的XML结构类型");
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failure($"XML解析错误", ex);
            }
        }

        /// <summary>
        /// XML结构是多个节点使用
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        public ResponseResult<IEnumerable<dynamic>> SelectNodes()
        {
            string uriPath = ReadPath;
            XElement xElement = uriPath.LoadFromFile().Root;
            IEnumerable<XElement> xElements = xElement.Descendants();
            if (xElements == null)
            {
                return ResponseResult<IEnumerable<dynamic>>.Failure($"Element '{uriPath}' not found.");
            }

            // 将XElement转换为匿名对象，避免直接暴露XElement
            var resultList = xElements.Select(element =>
            {
                // 创建包含元素名称和值的匿名对象
                return new
                {
                    Name = element.Name.LocalName,
                    Value = element.Value,
                    Attributes = element.Attributes().ToDictionary(attr => attr.Name.LocalName, attr => attr.Value),
                    Children = element.Elements().Select(child => new
                    {
                        Name = child.Name.LocalName,
                        Value = child.Value
                    }).ToList()
                } as dynamic;
            }).ToList();

            return ResponseResult<IEnumerable<dynamic>>.Success(resultList);
        }

        /// <summary>
        /// 存在多个节点，且通过类型T直接获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ResponseResult<IEnumerable<T>> SelectNodes<T>() where T : new()
        {
            try
            {
                if (_content == null)
                    return ResponseResult<IEnumerable<T>>.Failure("读取失败");

                var doc = XDocument.Parse(_content);
                var result = new List<T>();

                foreach (var node in doc.Root.Elements())
                {
                    var instance = new T();
                    FileHelper.MapXElementToObject(node, instance); // 自动映射
                    result.Add(instance);
                }

                return ResponseResult<IEnumerable<T>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResponseResult<IEnumerable<T>>.FromException(ex, $"解析失败: {ex.Message}");
            }
        }

        #endregion

        #region-- 读取Json数据 --

        public ResponseResult<IEnumerable<dynamic>> SelectKeys()
        {
            if (_content == null)
            {
                return ResponseResult<IEnumerable<dynamic>>.Failure($"读取失败");
            }
            List<dynamic> keys = new List<dynamic>();

            try
            {
                // 使用JToken.Parse()来解析不同类型的JSON
                JToken jsonToken = JToken.Parse(_content);

                // 递归方法，用于获取所有键
                void GetKeys(JToken token)
                {
                    if (token is JObject obj)
                    {
                        foreach (var property in obj.Properties())
                        {
                            keys.Add(new
                            {
                                Key = property.Name,
                                Type = property.Value.Type.ToString()
                            } as dynamic);
                            GetKeys(property.Value);
                        }
                    }
                    else if (token is JArray array)
                    {
                        // 处理根节点是数组的情况
                        keys.Add(new
                        {
                            Key = "RootArray",
                            Type = "Array",
                            Count = array.Count
                        } as dynamic);

                        foreach (var item in array)
                        {
                            GetKeys(item);
                        }
                    }
                    else if (token is JValue jValue)
                    {
                        // 处理根节点是值的情况
                        keys.Add(new
                        {
                            Key = "Root",
                            Type = jValue.Type.ToString()
                        } as dynamic);
                    }
                }

                GetKeys(jsonToken);
                return ResponseResult<IEnumerable<dynamic>>.Success(keys);
            }
            catch (Exception ex)
            {
                return ResponseResult<IEnumerable<dynamic>>.FromException(ex, "解析JSON失败");
            }
        }

        public ResponseResult<IEnumerable<dynamic>> SelectValues()
        {
            if (_content == null)
            {
                return ResponseResult<IEnumerable<dynamic>>.Failure($"读取失败");
            }
            List<dynamic> values = new List<dynamic>();

            try
            {
                // 使用JToken.Parse()来解析不同类型的JSON
                JToken jsonToken = JToken.Parse(_content);

                // 递归方法，用于获取所有值
                void GetValues(JToken token)
                {
                    if (token is JObject obj)
                    {
                        foreach (var property in obj.Properties())
                        {
                            if (property.Value is JValue jValue)
                            {
                                values.Add(new
                                {
                                    Key = property.Name,
                                    Value = jValue.Value,
                                    Type = jValue.Type.ToString()
                                } as dynamic);
                            }
                            GetValues(property.Value);
                        }
                    }
                    else if (token is JArray array)
                    {
                        // 处理根节点是数组的情况
                        values.Add(new
                        {
                            Key = "RootArray",
                            Value = $"Array with {array.Count} items",
                            Type = "Array",
                            Count = array.Count
                        } as dynamic);

                        foreach (var item in array)
                        {
                            GetValues(item);
                        }
                    }
                    else if (token is JValue jValue)
                    {
                        // 处理根节点是值的情况
                        values.Add(new
                        {
                            Key = "Root",
                            Value = jValue.Value,
                            Type = jValue.Type.ToString()
                        } as dynamic);
                    }
                }

                GetValues(jsonToken);
                return ResponseResult<IEnumerable<dynamic>>.Success(values);
            }
            catch (Exception ex)
            {
                return ResponseResult<IEnumerable<dynamic>>.FromException(ex, "解析JSON失败");
            }
        }

        /// <summary>
        /// 跟据key读取指定value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <summary>
        /// 跟据key读取指定value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ResponseResult SelectValue(string key)
        {
            if (_content == null)
            {
                return ResponseResult.Failure($"读取失败");
            }

            try
            {
                // 使用JToken.Parse()来解析不同类型的JSON
                JToken jsonToken = JToken.Parse(_content);
                JToken valueToken = null;

                // 根据JSON类型进行不同的查找
                if (jsonToken is JObject jsonObject)
                {
                    // 尝试直接查找key
                    valueToken = jsonObject.SelectToken(key);

                    // 如果没找到，尝试在根目录下查找
                    if (valueToken == null)
                    {
                        valueToken = jsonObject.Property(key)?.Value;
                    }
                }
                else if (jsonToken is JArray jsonArray)
                {
                    // 处理数组类型，支持索引访问，如 "[0].Name"
                    valueToken = jsonArray.SelectToken(key);
                }
                else if (jsonToken is JValue jsonValue)
                {
                    // 处理根节点是值的情况
                    if (string.Equals(key, "Root", StringComparison.OrdinalIgnoreCase))
                    {
                        valueToken = jsonValue;
                    }
                }

                if (valueToken == null || valueToken.Type == JTokenType.Null)
                {
                    // 键不存在或值为 null
                    return ResponseResult.Failure("转换失败，JToken为null");
                }
                return ResponseResult.Success(value: valueToken.ToString());
            }
            catch (Exception ex)
            {
                return ResponseResult.FromException(ex, "解析JSON失败");
            }
        }

        public ResponseResult<IEnumerable<T>> SelectObjects<T>() where T : new()
        {
            if (_content == null)
            {
                return ResponseResult<IEnumerable<T>>.Failure($"读取失败");
            }

            try
            {
                // 使用JToken.Parse()来解析不同类型的JSON
                JToken jsonToken = JToken.Parse(_content);
                List<T> resultList = new List<T>();

                if (jsonToken is JObject jsonObject)
                {
                    // 处理单个对象的情况
                    T instance = JsonConvert.DeserializeObject<T>(_content);
                    resultList.Add(instance);
                }
                else if (jsonToken is JArray jsonArray)
                {
                    // 处理数组的情况
                    resultList = JsonConvert.DeserializeObject<List<T>>(_content);
                }
                else
                {
                    // 处理其他类型，返回失败
                    return ResponseResult<IEnumerable<T>>.Failure("JSON格式不支持对象映射");
                }

                return ResponseResult<IEnumerable<T>>.Success(resultList);
            }
            catch (Exception ex)
            {
                return ResponseResult<IEnumerable<T>>.FromException(ex, "JSON对象映射失败");
            }
        }

        #endregion

        #region-- 读取Config数据--

        /// <summary>
        /// 读取WPF自带的App.config
        /// 这两个引用没有传递值，是读取config的值，所以不需要使用ref，
        /// 使用了struct后，如果传递数据的扩展方法，需要加上ref
        /// </summary>
        /// <param name="section"></param>
        public ResponseResult<TangdaoSortedDictionary<string, string>> SelectAppSection(string section)
        {
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);
            TangdaoSortedDictionary<string, string> dict = idict.Cast<DictionaryEntry>().ToTangdaoSortedDictionary();
            return ResponseResult<TangdaoSortedDictionary<string, string>>.Success(dict);
        }

        public ResponseResult SelectAppSection<T>(string section) where T : class, new()
        {
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);

            var dict = idict.Cast<DictionaryEntry>()
                           .ToDictionary(
                               de => de.Key.ToString(),
                               de => Convert.ChangeType(de.Value, typeof(T)) as T);

            return ResponseResult<Dictionary<string, T>>.Success(dict);
        }

        public ResponseResult SelectConfigByJsonConvert<T>(string section) where T : class, new()
        {
            T Target = new T();
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);
            var dict = idict.Cast<DictionaryEntry>()
                   .ToDictionary(
                       de => de.Key.ToString(),
                       de => JsonConvert.DeserializeObject<T>(de.Value.ToString())
                   );

            return ResponseResult<Dictionary<string, T>>.Success(dict);
        }

        /// <summary>
        /// 读取自定义的config文件
        /// </summary>
        /// <param name="menuList"></param>
        public ResponseResult<Dictionary<string, string>> SelectSection(string section)
        {
            if (ReadPath == null)
            {
                throw new FormatException("自定义读取Config文件时,请先调用Read方法读取指定路径文件");
            }

            Dictionary<string, string> dicts = new Dictionary<string, string>();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = ReadPath
            };

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            var customSection = (TangdaoMenuSection)config.GetSection(section);
            if (customSection == null)
            {
                dicts.Add("null", null);
                return ResponseResult<Dictionary<string, string>>.Failure(null);
            }
            foreach (MenuElement menu in customSection.Menus)
            {
                dicts.TryAdd(menu.Title, menu.Value);
            }
            return ResponseResult<Dictionary<string, string>>.Success(dicts);
        }

        #endregion

        #region--读取Ini文件--

        public ResponseResult<IniConfig> SelectIni(string section)
        {
            var configs = IniParser.Parse(_content);
            if (configs != null)
            {
                if (section == null)
                {
                    return null;
                }
                return ResponseResult<IniConfig>.Success(data: configs[section]);
            }
            else
            {
                return ResponseResult<IniConfig>.Failure($" {configs} 不存在");
            }
        }

        #endregion
    }
}