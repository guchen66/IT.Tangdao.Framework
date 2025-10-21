using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 提供类型解析功能的静态类
    /// 支持字符串到各种常用类型的转换
    /// </summary>
    public static class TypeParser
    {
        /// <summary>
        /// 类型解析器查找表
        /// Key: 目标类型
        /// Value: 将字符串转换为该类型对象的委托
        /// </summary>
        private static readonly Dictionary<Type, Func<string, object>> _parsers =
            new Dictionary<Type, Func<string, object>>()
            {
                [typeof(int)] = s => int.Parse(s, CultureInfo.InvariantCulture),
                [typeof(long)] = s => long.Parse(s, CultureInfo.InvariantCulture),
                [typeof(short)] = s => short.Parse(s, CultureInfo.InvariantCulture),
                [typeof(byte)] = s => byte.Parse(s, CultureInfo.InvariantCulture),
                [typeof(bool)] = s => bool.Parse(s),
                [typeof(float)] = s => float.Parse(s, CultureInfo.InvariantCulture),
                [typeof(double)] = s => double.Parse(s, CultureInfo.InvariantCulture),
                [typeof(decimal)] = s => decimal.Parse(s, CultureInfo.InvariantCulture),
                [typeof(char)] = s => char.Parse(s),
                [typeof(string)] = s => s,
                [typeof(DateTime)] = s => DateTime.Parse(s, CultureInfo.InvariantCulture),
                [typeof(DateTimeOffset)] = s => DateTimeOffset.Parse(s, CultureInfo.InvariantCulture),
                [typeof(TimeSpan)] = s => TimeSpan.Parse(s, CultureInfo.InvariantCulture),
                [typeof(Guid)] = s => Guid.Parse(s),
                [typeof(Uri)] = s => new Uri(s, UriKind.RelativeOrAbsolute),
                [typeof(Version)] = s => Version.Parse(s)
            };

        /// <summary>
        /// 获取类型解析器查找表（只读）
        /// </summary>
        public static IReadOnlyDictionary<Type, Func<string, object>> Table => _parsers;

        /// <summary>
        /// 尝试将字符串转换为指定类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <param name="result">转换结果</param>
        /// <returns>转换是否成功</returns>
        public static bool TryParse<T>(string input, out T result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var targetType = typeof(T);

            if (_parsers.TryGetValue(targetType, out var parser))
            {
                try
                {
                    result = (T)parser(input);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 将字符串转换为指定类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="InvalidOperationException">当类型不支持或转换失败时抛出</exception>
        public static T Parse<T>(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input string cannot be null or empty", nameof(input));

            var targetType = typeof(T);

            if (_parsers.TryGetValue(targetType, out var parser))
            {
                try
                {
                    return (T)parser(input);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to parse '{input}' to type {targetType.Name}", ex);
                }
            }

            throw new InvalidOperationException($"No parser registered for type {targetType.Name}");
        }

        /// <summary>
        /// 注册自定义类型解析器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="parser">解析委托</param>
        public static void RegisterParser<T>(Func<string, T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            _parsers[typeof(T)] = s => parser(s);
        }

        /// <summary>
        /// 检查是否支持指定类型的解析
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果支持解析返回true，否则返回false</returns>
        public static bool CanParse(Type type)
        {
            return type != null && _parsers.ContainsKey(type);
        }

        /// <summary>
        /// 检查是否支持指定类型的解析
        /// </summary>
        /// <typeparam name="T">要检查的类型</typeparam>
        /// <returns>如果支持解析返回true，否则返回false</returns>
        public static bool CanParse<T>()
        {
            return CanParse(typeof(T));
        }
    }
}