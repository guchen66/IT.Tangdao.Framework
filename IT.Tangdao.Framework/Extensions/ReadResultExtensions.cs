using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Utilys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ReadResultExtensions
    {
        public static Dictionary<string, string> ToDictionary(this ReadResult result)
        {
            if (!result.IsSuccess)
                return new Dictionary<string, string>(StringComparer.Ordinal);
            var dicts = result.ToReadResult<Dictionary<string, string>>();
            return dicts.Data
                         .OrderBy(kv => kv.Key)
                         .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);
        }

        /// <summary>
        /// 从泛型字典结果→POCO
        /// </summary>
        public static T ToObject<T>(this ReadResult result) where T : new()
        {
            var dict = result.ToDictionary();

            return DictToObject.Convert<T>(dict);
        }

        public static List<string> ToList(this ReadResult result, string keyValue = null)
        {
            var dict = result.ToDictionary();

            if (string.IsNullOrEmpty(keyValue) || string.Equals(keyValue, "value", StringComparison.OrdinalIgnoreCase))
            {
                return dict.Values.ToList();
            }

            if (string.Equals(keyValue, "key", StringComparison.OrdinalIgnoreCase))
            {
                return dict.Keys.ToList();
            }

            // 如果传入其他值，可以选择抛出异常或返回默认值
            return dict.Values.ToList(); // 或者抛出 ArgumentException
        }
    }
}