using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Abstractions.Results;
using System.Collections.ObjectModel;
using IT.Tangdao.Framework.Configurations;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ResponseResultExtension
    {
        public static IEnumerable<T> ToList<T>(this ResponseResult<TangdaoSortedDictionary<string, string>> queryableResult, Func<string, T> selector)
        {
            if (queryableResult == null) TangdaoGuards.ThrowIfNull(queryableResult);
            if (selector == null) TangdaoGuards.ThrowIfNull(selector);
            if (queryableResult.Data is TangdaoSortedDictionary<string, string> result)
            {
                return result.Values.Select(selector).ToList();
            }
            throw new ArgumentException();
        }

        /*---------- 1. 用户什么类型都不想建 ----------*/

        // 自动生成“只有 MenuName 属性”的匿名代理类
        public static List<dynamic> ToList(this ResponseResult<TangdaoSortedDictionary<string, string>> result)
            => result.Data.Values.Select(v => new { MenuName = v }).Cast<dynamic>().ToList();

        public static ObservableCollection<dynamic> ToObservableCollection(this ResponseResult<TangdaoSortedDictionary<string, string>> result)
            => new ObservableCollection<dynamic>(ToList(result));

        public static TangdaoSortedDictionary<string, string> ToDictionary(this ResponseResult result)
        {
            if (!result.IsSuccess)
                TangdaoGuards.ThrowIfNull(result);
            var dicts = result.ToGenericResult<TangdaoSortedDictionary<string, string>>();
            return dicts.Data;
        }

        /// <summary>
        /// 从泛型字典结果→POCO
        /// </summary>
        public static T ToObject<T>(this ResponseResult<TangdaoSortedDictionary<string, string>> result) where T : new()
        {
            if (!result.IsSuccess)
                TangdaoGuards.ThrowIfNull(result);
            if (result.Data is TangdaoSortedDictionary<string, string> dict)
            {
                return DictToObject.Convert<T>(dict);
            }
            throw new NotImplementedException();
        }

        public static List<string> ToList(this ResponseResult result, string keyValue = null)
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

        /// <summary>
        /// 针对Ini文件Key读取
        /// </summary>
        /// <param name="result"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string SelectKey(this ResponseResult<IniConfig> result, string key)
        {
            if (!result.IsSuccess)
                return null;

            result.Data.KeyValues.TryGetValue(key, out var value);
            return value;
        }
    }
}