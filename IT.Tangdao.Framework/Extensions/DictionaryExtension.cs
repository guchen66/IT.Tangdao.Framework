using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class DictionaryExtension
    {
        //此方法用于从字典中获取指定键的值，如果键不存在，则创建一个新的值并将其添加到字典中。
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key) where TValue : new()
        {
            if (!@this.ContainsKey(key))
            {
                @this[key] = new TValue();
            }

            return @this[key];
        }
        //此方法用于从字典中获取指定键的值，如果键不存在，则返回默认值
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            return @this.ContainsKey(key) ? @this[key] : default;
        }

        /// <summary>
        /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// </summary>
        public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false) dict.Add(key, value);
            return dict;
        }
        /// <summary>
        /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
        /// </summary>
        public static Dictionary<TKey, TValue> AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
            return dict;
        }
    }

}
