using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Abstractions.Results;
using System.Collections.ObjectModel;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ResponseResultExtension
    {
        public static IEnumerable<T> ToList<T>(this ResponseResult queryableResult, Func<string, T> selector)
        {
            if (queryableResult == null) TangdaoGuards.ThrowIfNull(queryableResult);
            if (selector == null) TangdaoGuards.ThrowIfNull(selector);
            if (queryableResult.Payload is TangdaoSortedDictionary<string, string> result)
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

        public static ReadOnlyCollection<dynamic> ToReadOnlyCollection(this ResponseResult<TangdaoSortedDictionary<string, string>> result)
            => new ReadOnlyCollection<dynamic>(ToList(result));

        /*---------- 2. 用户有自己的类型 T，且 T 有无参构造函数 ----------*/
        // 约定：T 必须有个 string MenuName { get; init; } 或 { get; set; }
        //public static List<T> ToList<T>(this ResponseResult<TangdaoSortedDictionary<string, string>> result)
        //    where T : new()
        //    => result.Data.Values.Select(v => Build<T>(v)).ToList();

        //public static ObservableCollection<T> ToObservableCollection<T>(
        //    this ResponseResult<TangdaoSortedDictionary<string, string>> result)
        //    where T : new()
        //    => new ObservableCollection<T>(ToList<T>(result));

        //public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(
        //    this ResponseResult<TangdaoSortedDictionary<string, string>> result)
        //    where T : new()
        //    => new ReadOnlyCollection<T>(ToList<T>(result));

        /*---------- 3. 用户想自己控制对象怎么造 ----------*/

        public static List<T> ToList<T>(this ResponseResult<TangdaoSortedDictionary<string, string>> result, Func<string, T> selector)
            => result.Data.Values.Select(selector).ToList();

        public static ObservableCollection<T> ToObservableCollection<T>(this ResponseResult<TangdaoSortedDictionary<string, string>> result, Func<string, T> selector)
            => new ObservableCollection<T>(ToList(result, selector));

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this ResponseResult<TangdaoSortedDictionary<string, string>> result, Func<string, T> selector)
            => new ReadOnlyCollection<T>(ToList(result, selector));

        /*---------- 内部小工具 ----------*/
        //private static T Build<T>(string menuName) where T : new()
        //{
        //    var t = new T();
        //    var prop = typeof(T).GetProperty("MenuName");
        //    if (prop is { CanWrite: true })
        //        prop.SetValue(t, menuName);
        //    else
        //        throw new InvalidOperationException($"类型 {typeof(T).Name} 必须包含可写的 string MenuName 属性。");
        //    return t;
        //}

        public static TangdaoSortedDictionary<string, string> ToDictionary(this ResponseResult result)
        {
            if (!result.IsSuccess)
                return new TangdaoSortedDictionary<string, string>(StringComparer.Ordinal);
            var dicts = result.ToGenericResult<TangdaoSortedDictionary<string, string>>();
            return dicts.Data;
        }

        /// <summary>
        /// 从泛型字典结果→POCO
        /// </summary>
        public static T ToObject<T>(this ResponseResult result) where T : new()
        {
            var dict = result.ToDictionary();

            return DictToObject.Convert<T>(dict);
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
    }
}