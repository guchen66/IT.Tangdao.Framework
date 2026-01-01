using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Abstractions.Results;
using System.Collections.ObjectModel;
using IT.Tangdao.Framework.Configurations;
using System.Windows;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ResponseResultExtension
    {
        #region 映射

        public static List<TResult> Select<T, TResult>(this IResponseResult<IEnumerable<T>> source, Func<T, TResult> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Select(selector).ToList();
        }

        public static List<T> Where<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Where(predicate).ToList();
        }

        public static List<T> ToList<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.ToList();
        }

        #endregion 映射

        #region Any / All

        public static bool Any<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Any();
        }

        public static bool Any<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Any(predicate);
        }

        public static bool All<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.All(predicate);
        }

        #endregion Any / All

        #region 数量 / 聚合

        public static int Count<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Count();
        }

        public static int Count<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Count(predicate);
        }

        public static T First<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.First();
        }

        public static T First<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.First(predicate);
        }

        public static T FirstOrDefault<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.FirstOrDefault();
        }

        public static T FirstOrDefault<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.FirstOrDefault(predicate);
        }

        public static T Last<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Last();
        }

        public static T Last<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Last(predicate);
        }

        public static T LastOrDefault<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.LastOrDefault();
        }

        public static T LastOrDefault<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.LastOrDefault(predicate);
        }

        public static T Single<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Single();
        }

        public static T Single<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Single(predicate);
        }

        public static T SingleOrDefault<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.SingleOrDefault();
        }

        public static T SingleOrDefault<T>(this IResponseResult<IEnumerable<T>> source, Func<T, bool> predicate)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.SingleOrDefault(predicate);
        }

        #endregion 数量 / 聚合

        #region 元素级

        public static List<T> Take<T>(this IResponseResult<IEnumerable<T>> source, int count)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Take(count).ToList();
        }

        public static List<T> Skip<T>(this IResponseResult<IEnumerable<T>> source,
            int count)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Skip(count).ToList();
        }

        public static List<T> Distinct<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Distinct().ToList();
        }

        public static List<T> OrderBy<T, TKey>(this IResponseResult<IEnumerable<T>> source, Func<T, TKey> keySelector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.OrderBy(keySelector).ToList();
        }

        public static List<T> OrderByDescending<T, TKey>(this IResponseResult<IEnumerable<T>> source, Func<T, TKey> keySelector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.OrderByDescending(keySelector).ToList();
        }

        public static List<T> Reverse<T>(this IResponseResult<IEnumerable<T>> source)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.AsEnumerable().Reverse().ToList();
        }

        #endregion 元素级

        #region 聚合

        public static TResult Max<T, TResult>(this IResponseResult<IEnumerable<T>> source, Func<T, TResult> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Max(selector);
        }

        public static TResult Min<T, TResult>(this IResponseResult<IEnumerable<T>> source, Func<T, TResult> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Min(selector);
        }

        public static decimal Sum<T>(this IResponseResult<IEnumerable<T>> source, Func<T, decimal> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Sum(selector);
        }

        public static int Sum<T>(this IResponseResult<IEnumerable<T>> source, Func<T, int> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Sum(selector);
        }

        public static double Sum<T>(this IResponseResult<IEnumerable<T>> source, Func<T, double> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Sum(selector);
        }

        public static float Sum<T>(this IResponseResult<IEnumerable<T>> source, Func<T, float> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Sum(selector);
        }

        public static long Sum<T>(this IResponseResult<IEnumerable<T>> source, Func<T, long> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Sum(selector);
        }

        public static decimal Average<T>(this IResponseResult<IEnumerable<T>> source, Func<T, decimal> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Average(selector);
        }

        public static double Average<T>(this IResponseResult<IEnumerable<T>> source, Func<T, double> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Average(selector);
        }

        public static float Average<T>(this IResponseResult<IEnumerable<T>> source, Func<T, float> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Average(selector);
        }

        public static double Average<T>(this IResponseResult<IEnumerable<T>> source, Func<T, int> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Average(selector);
        }

        public static double Average<T>(this IResponseResult<IEnumerable<T>> source, Func<T, long> selector)
        {
            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentNullException(nameof(source));
            return source.Data.Average(selector);
        }

        #endregion 聚合

        /// <summary>
        /// 针对字典特殊处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<T> ToList<T>(this IResponseResult<IDictionary<string, string>> source, Func<KeyValuePair<string, string>, T> selector)
        {
            TangdaoGuards.ThrowIfNull(source);
            TangdaoGuards.ThrowIfNull(selector);

            if (!source.IsSuccess || source.Data == null)
                throw new ArgumentException("数据源为空或失败");

            // 整个字典就是 IEnumerable<KeyValuePair<,>>
            return source.Data.Select(selector);
        }

        /// <summary>
        /// 从泛型字典结果→POCO
        /// </summary>
        public static T ToObject<T>(this IResponseResult<TangdaoSortedDictionary<string, string>> result) where T : new()
        {
            if (!result.IsSuccess)
                TangdaoGuards.ThrowIfNull(result);
            if (result.Data is TangdaoSortedDictionary<string, string> dict)
            {
                return DictToObject.Convert<T>(dict);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 针对Ini文件Key读取
        /// </summary>
        /// <param name="result"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string SelectKey(this IResponseResult<IniConfig> result, string key)
        {
            if (!result.IsSuccess)
                return null;

            result.Data.KeyValues.TryGetValue(key, out var value);
            return value;
        }
    }
}