using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoSortedDictionaryExtension
    {
        /// <summary>
        /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// </summary>
        public static TangdaoSortedDictionary<TKey, TValue> TryAdd<TKey, TValue>(this TangdaoSortedDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false) dict.Add(key, value);
            return dict;
        }

        /// <summary>
        /// 更新并修改
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="addValue"></param>
        /// <param name="updateValueFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddOrUpdate<TKey, TValue>(this TangdaoSortedDictionary<TKey, TValue> dict, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (updateValueFactory == null) TangdaoGuards.ThrowIfNull(nameof(updateValueFactory));
            dict[key] = dict.TryGetValue(key, out var oldVal) ? updateValueFactory(key, oldVal) : addValue;
        }

        /// <summary>
        /// 将 TangdaoSortedDictionary 转换为 Dictionary
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this TangdaoSortedDictionary<TKey, TValue> dict)
        {
            if (dict == null) TangdaoGuards.ThrowIfNull(nameof(dict));

            return dict.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>
        /// 将 TangdaoSortedDictionary 转换为 Dictionary，并使用指定的键选择器和值选择器
        /// </summary>
        public static Dictionary<TResultKey, TResultValue> ToDictionary<TKey, TValue, TResultKey, TResultValue>(
            this TangdaoSortedDictionary<TKey, TValue> dict,
            Func<KeyValuePair<TKey, TValue>, TResultKey> keySelector,
            Func<KeyValuePair<TKey, TValue>, TResultValue> valueSelector)
        {
            if (dict == null) TangdaoGuards.ThrowIfNull(nameof(dict));
            if (keySelector == null) TangdaoGuards.ThrowIfNull(nameof(keySelector));
            if (valueSelector == null) TangdaoGuards.ThrowIfNull(nameof(valueSelector));

            return dict.ToDictionary(keySelector, valueSelector);
        }

        /// <summary>
        /// 将 TangdaoSortedDictionary 转换为 ConcurrentDictionary
        /// </summary>
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this TangdaoSortedDictionary<TKey, TValue> dict)
        {
            if (dict == null) TangdaoGuards.ThrowIfNull(nameof(dict));

            return new ConcurrentDictionary<TKey, TValue>(dict);
        }

        /// <summary>
        /// 将 TangdaoSortedDictionary 转换为 ConcurrentDictionary，并使用指定的键选择器和值选择器
        /// </summary>
        public static ConcurrentDictionary<TResultKey, TResultValue> ToConcurrentDictionary<TKey, TValue, TResultKey, TResultValue>(
            this TangdaoSortedDictionary<TKey, TValue> dict,
            Func<KeyValuePair<TKey, TValue>, TResultKey> keySelector,
            Func<KeyValuePair<TKey, TValue>, TResultValue> valueSelector)
        {
            if (dict == null) TangdaoGuards.ThrowIfNull(nameof(dict));
            if (keySelector == null) TangdaoGuards.ThrowIfNull(nameof(keySelector));
            if (valueSelector == null) TangdaoGuards.ThrowIfNull(nameof(valueSelector));

            var keyValuePairs = dict.Select(kv => new KeyValuePair<TResultKey, TResultValue>(keySelector(kv), valueSelector(kv)));
            return new ConcurrentDictionary<TResultKey, TResultValue>(keyValuePairs);
        }
    }
}