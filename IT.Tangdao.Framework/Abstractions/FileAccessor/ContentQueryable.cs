using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Selectors;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using IT.Tangdao.Framework.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    internal sealed class ContentQueryable : IContentQueryable, IContentXmlQueryable, IContentJsonQueryable, IContentConfigQueryable
    {
        #region--索引器--

        public int ReadIndex
        {
            get => _readIndex;
            set => _readIndex = value;
        }

        private int _readIndex = -1;

        // 实现接口中的索引器
        public IContentQueryable this[int readIndex]
        {
            get
            {
                _readIndex = readIndex;
                return this;
            }
        }

        public string ReadObject
        {
            get => _readObject;
            set => _readObject = value;
        }

        private string _readObject;

        // 实现接口中的索引器
        public IContentQueryable this[string readObject]
        {
            get
            {
                _readObject = readObject;
                return this;
            }
        }

        #endregion
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(ContentQueryable));
        private string _content = string.Empty;

        public string Content
        {
            get => _content;
            set => _content = value;
        }

        private DaoFileType _detected = DaoFileType.None;

        /* ========== IContentQueryable 通用方法 **只写一次** ========== */

        public IContentQueryable Read(string path, DaoFileType t)
        {
            _content = File.ReadAllText(path);
            _detected = t == DaoFileType.None ? FileSelector.DetectFromContent(_content) : t;

            // ① 根 key：路径 + 格式
            var rootKey = string.Format("Content:{0}:{1}", path, _detected);
            TangdaoParameter tangdaoParameter = new TangdaoParameter();
            tangdaoParameter.Add(rootKey, _content);    //缓存内容
            TangdaoContext.SetTangdaoParameter(rootKey, tangdaoParameter);
            return this;
        }

        public async Task<IContentQueryable> ReadAsync(string path, DaoFileType daoFileType = DaoFileType.None)
        {
            _content = await ReadTxtAsync(path);
            _detected = daoFileType == DaoFileType.None ? FileSelector.DetectFromContent(_content) : daoFileType;
            var rootKey = string.Format("Content:{0}:{1}", path, _detected);
            TangdaoParameter tangdaoParameter = new TangdaoParameter();
            tangdaoParameter.Add(rootKey, _content);
            TangdaoContext.SetTangdaoParameter(rootKey, tangdaoParameter);
            TangdaoContext.SetInstance(rootKey);
            return this;
        }

        private static async Task<string> ReadTxtAsync(string path)
        {
            using (var stream = new StreamReader(path.UseFileOpenRead()))
            {
                return await stream.ReadToEndAsync();
            }
        }

        public IContentQueryable Auto() => this;   // 逻辑已在内置字段，直接返回自己

        /* ========== 格式切换 ========== */

        public IContentXmlQueryable AsXml() => this;

        public IContentJsonQueryable AsJson() => this;

        public IContentConfigQueryable AsConfig() => this;

        #region--XML读取--

        public ReadResult SelectNode(string node)
        {
            if (_content == null)
            {
                return ReadResult.Failure("文件未解析成功");
            }

            try
            {
                var doc = XDocument.Parse(_content);
                var root = doc.RootElement();
                var xmlType = FileSelector.DetectXmlStructure(doc);

                switch (xmlType)
                {
                    case DaoXmlType.Empty:
                        return ReadResult.Failure("XML内容为空");

                    case DaoXmlType.None:
                        return ReadResult.Failure("XML只有声明没有内容");

                    case DaoXmlType.Single:
                        // 单节点结构 - 直接查找目标节点
                        var singleElement = root.Element(node) ?? root.Elements().First().Element(node);
                        if (singleElement == null)
                        {
                            return ReadResult.Failure($"节点 '{node}' 不存在");
                        }
                        return ReadResult.Success(singleElement.Value);

                    case DaoXmlType.Multiple:
                        if (ReadIndex < 0)
                        {
                            // 尝试直接查找节点(适用于扁平结构)
                            var directElement = root.Element(node);
                            if (directElement != null)
                            {
                                return ReadResult.Success(directElement.Value);
                            }
                            return ReadResult.Failure("存在多个节点，请指定索引");
                        }
                        else
                        {
                            // 处理指定索引的情况
                            var parentNode = root.Elements().ElementAtOrDefault(ReadIndex);
                            if (parentNode == null)
                            {
                                return ReadResult.Failure($"索引 {ReadIndex} 超出范围");
                            }

                            var targetElement = parentNode.Element(node);
                            if (targetElement == null)
                            {
                                return ReadResult.Failure($"子节点 '{node}' 不存在");
                            }

                            return ReadResult.Success(targetElement.Value);
                        }

                    default:
                        return ReadResult.Failure("未知的XML结构类型");
                }
            }
            catch (Exception ex)
            {
                return ReadResult.Failure($"XML解析错误: {ex.Message}");
            }
            finally
            {
                _readIndex = -1;
            }
        }

        /// <summary>
        /// 这里的path是uri地址，不是XML具体数据
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        public ReadResult SelectNodes(string uriPath)
        {
            XElement xElement = uriPath.LoadFromFile().Root;
            IEnumerable<XElement> xElements = xElement.Descendants();
            if (xElements == null)
            {
                return ReadResult.Failure($"Element '{uriPath}' not found.");
            }
            return ReadResult<IEnumerable<XElement>>.Success(xElements);
        }

        public ReadResult<List<T>> SelectNodes<T>(string rootElement, Func<XElement, T> selector)
        {
            try
            {
                if (_content == null)
                {
                    return ReadResult<List<T>>.Failure($"读取失败");
                }
                var doc = XDocument.Parse(_content);
                var elements = doc.Root.Elements().Select(node => node).ToList();

                if (elements == null || elements.Count == 0)
                {
                    return ReadResult<List<T>>.Failure("未找到指定的元素。");
                }

                List<T> result = elements.Select(selector).ToList();
                return ReadResult<List<T>>.Success(result);
            }
            catch (Exception ex)
            {
                return ReadResult<List<T>>.FromException(ex, $"解析 XML 失败: {ex.Message}");
            }
        }

        public ReadResult<List<T>> SelectNodes<T>() where T : new()
        {
            try
            {
                if (_content == null)
                    return ReadResult<List<T>>.Failure("读取失败");

                var doc = XDocument.Parse(_content);
                var result = new List<T>();

                foreach (var node in doc.Root.Elements())
                {
                    var instance = new T();
                    FileSelector.MapXElementToObject(node, instance); // 自动映射
                    result.Add(instance);
                }

                return ReadResult<List<T>>.Success(result);
            }
            catch (Exception ex)
            {
                return ReadResult<List<T>>.FromException(ex, $"解析失败: {ex.Message}");
            }
        }

        #endregion

        #region-- 读取Json数据 --

        public ReadResult SelectKeys()
        {
            if (_content == null)
            {
                return ReadResult.Failure($"读取失败");
            }
            List<string> keys = new List<string>();
            JObject jsonObject = JObject.Parse(_content);

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
            return ReadResult<List<string>>.Success(keys);
        }

        /// <summary>
        /// 跟据key读取指定value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ReadResult SelectValue(string key)
        {
            if (_content == null)
            {
                return ReadResult.Failure($"读取失败");
            }
            var path = DirectoryHelper.SelectDirectoryByName(_content);
            string jsonContent = File.ReadAllText(path);
            JObject jsonObject = JObject.Parse(jsonContent);
            if (ReadObject == null)
            {
                return ReadResult.Failure("转换失败，未设置索引器");
            }
            JToken valueToken = jsonObject.SelectToken($"{ReadObject}.{key}");

            if (valueToken == null || valueToken.Type == JTokenType.Null)
            {
                // 键不存在或值为 null
                return ReadResult.Failure("转换失败，JToken为null");
            }
            return ReadResult.Success(valueToken.ToString());
        }

        #endregion

        #region-- 读取Config数据--

        /// <summary>
        /// 读取WPF自带的App.config
        /// 这两个引用没有传递值，是读取config的值，所以不需要使用ref，
        /// 使用了struct后，如果传递数据的扩展方法，需要加上ref
        /// </summary>
        /// <param name="section"></param>
        public ReadResult SelectConfig(string section)
        {
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);
            TangdaoSortedDictionary<string, string> dict = idict.Cast<DictionaryEntry>().ToTangdaoSortedDictionary();

            return ReadResult<TangdaoSortedDictionary<string, string>>.Success(dict);
        }

        public ReadResult SelectConfig<T>(string section) where T : class, new()
        {
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);

            var dict = idict.Cast<DictionaryEntry>()
                           .ToDictionary(
                               de => de.Key.ToString(),
                               de => Convert.ChangeType(de.Value, typeof(T)) as T
                           );

            return ReadResult<Dictionary<string, T>>.Success(dict);
        }

        public ReadResult SelectConfigByJsonConvert<T>(string section) where T : class, new()
        {
            T Target = new T();
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);
            var dict = idict.Cast<DictionaryEntry>()
                   .ToDictionary(
                       de => de.Key.ToString(),
                       de => JsonConvert.DeserializeObject<T>(de.Value.ToString())
                   );

            return ReadResult<Dictionary<string, T>>.Success(dict);
        }

        /// <summary>
        /// 读取自定义的config文件
        /// </summary>
        /// <param name="menuList"></param>
        public ReadResult SelectCustomConfig(string configName, string section)
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
                return ReadResult<string>.Failure(null);
            }
            foreach (MenuElement menu in customSection.Menus)
            {
                dicts.TryAdd(menu.Title, menu.Value);
            }
            return ReadResult<Dictionary<string, string>>.Success(dicts);
        }

        #endregion

        protected internal DaoFileType DetectedType => _detected;
    }
}