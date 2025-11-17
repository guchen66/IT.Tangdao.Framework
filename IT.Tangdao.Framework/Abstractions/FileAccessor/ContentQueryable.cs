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

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    internal sealed class ContentQueryable : IContentQueryable, IContentXmlQueryable, IContentJsonQueryable, IContentConfigQueryable, IContentIniQueryable
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

        internal string _path = string.Empty;

        internal DaoFileType DetectedType => _detected;
        private DaoFileType _detected = DaoFileType.None;

        /* ========== IContentQueryable 通用方法 **只写一次** ========== */

        public IContentQueryable Read(string path, DaoFileType t)
        {
            _content = File.ReadAllText(path);
            _path = path;
            _detected = t == DaoFileType.None ? FileHelper.DetectFromContent(_content) : t;
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
            _detected = daoFileType == DaoFileType.None ? FileHelper.DetectFromContent(_content) : daoFileType;
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

        public IContentXmlQueryable AsXml()
        {
            if (_detected == DaoFileType.Xml)
            {
                return this;
            }
            throw new FormatException("转换失败，读取的内容不是有效的XML格式");
        }

        public IContentJsonQueryable AsJson()
        {
            if (_detected == DaoFileType.Json)
            {
                return this;
            }
            throw new FormatException("转换失败，读取的内容不是有效的Json格式");
        }

        public IContentConfigQueryable AsConfig()
        {
            // 如果是 Config 类型，或者是未读取状态（用于读取默认 App.config），都允许转换
            if (_detected == DaoFileType.Config || _detected == DaoFileType.None)
            {
                return this;
            }
            throw new FormatException("转换失败，读取的内容不是有效的Config格式");
        }

        public IContentIniQueryable AsIni()
        {
            if (_detected == DaoFileType.Ini)
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
                        if (ReadIndex < 0)
                        {
                            // 尝试直接查找节点(适用于扁平结构)
                            var directElement = root.Element(node);
                            if (directElement != null)
                            {
                                return ResponseResult.Success(value: directElement.Value);
                            }
                            return ResponseResult.Failure("存在多个节点，请指定索引", new XmlException($"存在多个节点，请指定索引，目标节点: {node}"));
                        }
                        else
                        {
                            // 处理指定索引的情况
                            var parentNode = root.Elements().ElementAtOrDefault(ReadIndex);
                            if (parentNode == null)
                            {
                                return ResponseResult.Failure($"索引 {ReadIndex} 超出范围", new XmlException($"索引 {ReadIndex} 超出范围"));
                            }

                            var targetElement = parentNode.Element(node);
                            if (targetElement == null)
                            {
                                return ResponseResult.Failure($"子节点 '{node}' 不存在", new XmlException($"子节点 '{node}' 不存在"));
                            }

                            return ResponseResult.Success(value: targetElement.Value);
                        }

                    default:
                        return ResponseResult.Failure("未知的XML结构类型");
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failure($"XML解析错误", ex);
            }
            finally
            {
                _readIndex = -1;
            }
        }

        /// <summary>
        /// XML结构是多个节点使用
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        public ResponseResult SelectNodes()
        {
            string uriPath = _path;
            XElement xElement = uriPath.LoadFromFile().Root;
            IEnumerable<XElement> xElements = xElement.Descendants();
            if (xElements == null)
            {
                return ResponseResult.Failure($"Element '{uriPath}' not found.");
            }
            return ResponseResult<IEnumerable<XElement>>.Success(xElements);
        }

        public ResponseResult<List<T>> SelectNodes<T>(string rootElement, Func<XElement, T> selector)
        {
            try
            {
                if (_content == null)
                {
                    return ResponseResult<List<T>>.Failure($"读取失败");
                }
                var doc = XDocument.Parse(_content);
                var elements = doc.Root.Elements().Select(node => node).ToList();

                if (elements == null || elements.Count == 0)
                {
                    return ResponseResult<List<T>>.Failure("未找到指定的元素。");
                }

                List<T> result = elements.Select(selector).ToList();
                return ResponseResult<List<T>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResponseResult<List<T>>.FromException(ex, $"解析 XML 失败: {ex.Message}");
            }
        }

        public ResponseResult<List<T>> SelectNodes<T>() where T : new()
        {
            try
            {
                if (_content == null)
                    return ResponseResult<List<T>>.Failure("读取失败");

                var doc = XDocument.Parse(_content);
                var result = new List<T>();

                foreach (var node in doc.Root.Elements())
                {
                    var instance = new T();
                    FileHelper.MapXElementToObject(node, instance); // 自动映射
                    result.Add(instance);
                }

                return ResponseResult<List<T>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResponseResult<List<T>>.FromException(ex, $"解析失败: {ex.Message}");
            }
        }

        #endregion

        #region-- 读取Json数据 --

        public ResponseResult SelectKeys()
        {
            if (_content == null)
            {
                return ResponseResult.Failure($"读取失败");
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
            return ResponseResult<List<string>>.Success(keys);
        }

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
            var path = DirectoryHelper.SelectDirectoryByName(_content);
            string jsonContent = File.ReadAllText(path);
            JObject jsonObject = JObject.Parse(jsonContent);
            if (ReadObject == null)
            {
                return ResponseResult.Failure("转换失败，未设置索引器");
            }
            JToken valueToken = jsonObject.SelectToken($"{ReadObject}.{key}");

            if (valueToken == null || valueToken.Type == JTokenType.Null)
            {
                // 键不存在或值为 null
                return ResponseResult.Failure("转换失败，JToken为null");
            }
            return ResponseResult.Success(value: valueToken.ToString());
        }

        #endregion

        #region-- 读取Config数据--

        /// <summary>
        /// 读取WPF自带的App.config
        /// 这两个引用没有传递值，是读取config的值，所以不需要使用ref，
        /// 使用了struct后，如果传递数据的扩展方法，需要加上ref
        /// </summary>
        /// <param name="section"></param>
        public ResponseResult<TangdaoSortedDictionary<string, string>> SelectAppConfig(string section)
        {
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);
            TangdaoSortedDictionary<string, string> dict = idict.Cast<DictionaryEntry>().ToTangdaoSortedDictionary();
            return ResponseResult<TangdaoSortedDictionary<string, string>>.Success(dict);
        }

        public ResponseResult SelectAppConfig<T>(string section) where T : class, new()
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
        public ResponseResult<Dictionary<string, string>> SelectCustomConfig(string configName, string section)
        {
            if (_detected != DaoFileType.Config)
            {
                throw new FormatException("自定义读取Config文件时,请先调用Read方法读取指定路径文件");
            }

            Dictionary<string, string> dicts = new Dictionary<string, string>();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = _path
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

        #region --AbsolutePath--

        public IContentQueryable Read(AbsolutePath path, DaoFileType t = DaoFileType.None)
        {
            _content = File.ReadAllText(path.Value);
            _detected = t == DaoFileType.None ? FileHelper.DetectFromContent(_content) : t;

            // ① 根 key：路径 + 格式
            var rootKey = $"Content:{path.Value}:{_detected}";
            TangdaoParameter tangdaoParameter = new TangdaoParameter();
            tangdaoParameter.Add(rootKey, _content);    //缓存内容
            TangdaoContext.SetTangdaoParameter(rootKey, tangdaoParameter);
            return this;
        }

        #endregion
    }
}