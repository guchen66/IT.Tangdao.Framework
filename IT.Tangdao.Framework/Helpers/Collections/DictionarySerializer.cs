using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Results;
using Newtonsoft.Json;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 字典序列化器，用于字典的序列化和反序列化
    /// </summary>
    public class DictionarySerializer<TKey, TValue>
    {
        private readonly TangdaoSortedDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// 初始化字典序列化器
        /// </summary>
        internal DictionarySerializer(TangdaoSortedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// 序列化为JSON字符串
        /// </summary>
        public string ToJson()
        {
            // 这里可以实现JSON序列化逻辑
            // 暂时返回简单的字符串表示
            return string.Join(", ", _dictionary.Select(kvp => $"\"{kvp.Key}\":\"{kvp.Value}\""));
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
        /// 序列化为XML字符串
        /// </summary>
        public string ToXml()
        {
            // 这里可以实现XML序列化逻辑
            var sb = new StringBuilder();
            sb.AppendLine("<Dictionary>");
            foreach (var kvp in _dictionary)
            {
                sb.AppendLine($"  <Item Key=\"{kvp.Key}\" Value=\"{kvp.Value}\" />");
            }
            sb.AppendLine("</Dictionary>");
            return sb.ToString();
        }

        /// <summary>
        /// 序列化为CSV字符串
        /// </summary>
        public string ToCsv(char separator = ',')
        {
            // 这里可以实现CSV序列化逻辑
            return string.Join(Environment.NewLine, _dictionary.Select(kvp => $"{kvp.Key}{separator}{kvp.Value}"));
        }

        /// <summary>
        /// 从JSON字符串反序列化
        /// </summary>
        public void FromJson(string json)
        {
            // 这里可以实现JSON反序列化逻辑
            // 暂时不实现具体逻辑
        }

        /// <summary>
        /// 从XML字符串反序列化
        /// </summary>
        public void FromXml(string xml)
        {
            // 这里可以实现XML反序列化逻辑
            // 暂时不实现具体逻辑
        }
    }
}