using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 提供类型解析功能的静态类
    /// 支持字符串到各种常用类型的转换，包括可空类型、枚举类型等
    /// 支持自动回退到 .NET 内置转换机制
    /// </summary>
    public static class TypeParser
    {
        /// <summary>
        /// 类型解析器查找表（线程安全）
        /// Key: 目标类型
        /// Value: 将字符串转换为该类型对象的委托，接受字符串和格式提供器参数
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Func<string, IFormatProvider, object>> _parsers =
            new ConcurrentDictionary<Type, Func<string, IFormatProvider, object>>();

        /// <summary>
        /// 静态构造函数，注册内置类型解析器
        /// </summary>
        static TypeParser()
        {
            RegisterBuiltInParsers();
        }

        /// <summary>
        /// 注册所有内置类型解析器
        /// </summary>
        private static void RegisterBuiltInParsers()
        {
            RegisterParser<int>((s, fp) => int.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<long>((s, fp) => long.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<short>((s, fp) => short.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<byte>((s, fp) => byte.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<bool>((s, _) => ParseBool(s));
            RegisterParser<float>((s, fp) => float.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<double>((s, fp) => double.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<decimal>((s, fp) => decimal.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<char>((s, _) => char.Parse(s));
            RegisterParser<string>((s, _) => s);
            RegisterParser<DateTime>((s, fp) => DateTime.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<DateTimeOffset>((s, fp) => DateTimeOffset.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<TimeSpan>((s, fp) => TimeSpan.Parse(s, fp ?? CultureInfo.InvariantCulture));
            RegisterParser<Guid>((s, _) => Guid.Parse(s));
            RegisterParser<Uri>((s, _) => new Uri(s, UriKind.RelativeOrAbsolute));
            RegisterParser<Version>((s, _) => Version.Parse(s));
        }

        /// <summary>
        /// 智能布尔值解析
        /// 支持多种表示形式：true/1/yes/on/是（不区分大小写）
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>解析后的布尔值</returns>
        private static bool ParseBool(string s)
        {
            return s.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("1", StringComparison.Ordinal) ||
                   s.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("是", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取类型解析器查找表（只读）
        /// </summary>
        public static IReadOnlyDictionary<Type, Func<string, IFormatProvider, object>> Table => _parsers;

        /// <summary>
        /// 尝试将字符串转换为指定类型（不带格式提供器）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <param name="result">转换结果</param>
        /// <returns>转换是否成功</returns>
        public static bool TryParse<T>(string input, out T result)
        {
            return TryParse(input, null, out result);
        }

        /// <summary>
        /// 尝试将字符串转换为指定类型（带格式提供器）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <param name="formatProvider">格式提供器</param>
        /// <param name="result">转换结果</param>
        /// <returns>转换是否成功</returns>
        public static bool TryParse<T>(string input, IFormatProvider formatProvider, out T result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var targetType = typeof(T);
            return TryParseCore(input, targetType, formatProvider, out var objResult) &&
                   objResult is T typedResult && (result = typedResult) != null;
        }

        /// <summary>
        /// 尝试将字符串转换为指定类型（不带格式提供器，非泛型版本）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="result">转换结果</param>
        /// <returns>转换是否成功</returns>
        public static bool TryParse(string input, Type targetType, out object result)
        {
            return TryParse(input, targetType, null, out result);
        }

        /// <summary>
        /// 尝试将字符串转换为指定类型（带格式提供器，非泛型版本）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="formatProvider">格式提供器</param>
        /// <param name="result">转换结果</param>
        /// <returns>转换是否成功</returns>
        public static bool TryParse(string input, Type targetType, IFormatProvider formatProvider, out object result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(input) || targetType == null)
                return false;

            return TryParseCore(input, targetType, formatProvider, out result);
        }

        /// <summary>
        /// 核心解析方法，包含完整的解析流程
        /// 解析顺序：自定义解析器 → 枚举类型 → Convert.ChangeType → TypeDescriptor
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="formatProvider">格式提供器</param>
        /// <param name="result">转换结果</param>
        /// <returns>转换是否成功</returns>
        private static bool TryParseCore(string input, Type targetType, IFormatProvider formatProvider, out object result)
        {
            result = null;

            // 处理可空类型，获取底层类型
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // 1. 优先使用自定义注册的解析器
            if (_parsers.TryGetValue(underlyingType, out var parser))
            {
                try
                {
                    result = parser(input, formatProvider);
                    return true;
                }
                catch { }
            }

            // 2. 处理枚举类型
            if (underlyingType.IsEnum)
            {
                try
                {
                    result = Enum.Parse(underlyingType, input, true);
                    return true;
                }
                catch { }
            }

            // 3. 使用 Convert.ChangeType
            try
            {
                result = Convert.ChangeType(input, underlyingType, formatProvider ?? CultureInfo.InvariantCulture);
                return true;
            }
            catch { }

            // 4. 使用 TypeDescriptor.GetConverter
            try
            {
                var converter = TypeDescriptor.GetConverter(underlyingType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    var culture = formatProvider as CultureInfo ?? CultureInfo.InvariantCulture;
                    result = converter.ConvertFromString(null, culture, input);
                    return true;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 将字符串转换为指定类型（不带格式提供器）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="ArgumentException">输入字符串为空时抛出</exception>
        /// <exception cref="InvalidOperationException">转换失败时抛出</exception>
        public static T Parse<T>(string input)
        {
            return Parse<T>(input, null);
        }

        /// <summary>
        /// 将字符串转换为指定类型（带格式提供器）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <param name="formatProvider">格式提供器</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="ArgumentException">输入字符串为空时抛出</exception>
        /// <exception cref="InvalidOperationException">转换失败时抛出</exception>
        public static T Parse<T>(string input, IFormatProvider formatProvider)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input string cannot be null or empty", nameof(input));

            if (TryParse(input, formatProvider, out T result))
                return result;

            throw new InvalidOperationException($"Failed to parse '{input}' to type {typeof(T).Name}");
        }

        /// <summary>
        /// 将字符串转换为指定类型（不带格式提供器，非泛型版本）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="ArgumentException">输入字符串为空时抛出</exception>
        /// <exception cref="InvalidOperationException">转换失败时抛出</exception>
        public static object Parse(string input, Type targetType)
        {
            return Parse(input, targetType, null);
        }

        /// <summary>
        /// 将字符串转换为指定类型（带格式提供器，非泛型版本）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="formatProvider">格式提供器</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="ArgumentException">输入字符串为空时抛出</exception>
        /// <exception cref="InvalidOperationException">转换失败时抛出</exception>
        public static object Parse(string input, Type targetType, IFormatProvider formatProvider)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input string cannot be null or empty", nameof(input));

            if (TryParse(input, targetType, formatProvider, out var result))
                return result;

            throw new InvalidOperationException($"Failed to parse '{input}' to type {targetType.Name}");
        }

        /// <summary>
        /// 将字符串转换为指定类型，失败时返回默认值（不带格式提供器）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <returns>转换结果或默认值</returns>
        public static T ParseOrDefault<T>(string input)
        {
            return ParseOrDefault(input, default(T));
        }

        /// <summary>
        /// 将字符串转换为指定类型，失败时返回指定默认值（不带格式提供器）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换结果或默认值</returns>
        public static T ParseOrDefault<T>(string input, T defaultValue)
        {
            return ParseOrDefault(input, defaultValue, null);
        }

        /// <summary>
        /// 将字符串转换为指定类型，失败时返回指定默认值（带格式提供器）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="formatProvider">格式提供器</param>
        /// <returns>转换结果或默认值</returns>
        public static T ParseOrDefault<T>(string input, T defaultValue, IFormatProvider formatProvider)
        {
            return TryParse(input, formatProvider, out T result) ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为指定类型，失败时返回默认值（不带格式提供器，非泛型版本）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换结果或默认值</returns>
        public static object ParseOrDefault(string input, Type targetType, object defaultValue = null)
        {
            return ParseOrDefault(input, targetType, defaultValue, null);
        }

        /// <summary>
        /// 将字符串转换为指定类型，失败时返回指定默认值（带格式提供器，非泛型版本）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="formatProvider">格式提供器</param>
        /// <returns>转换结果或默认值</returns>
        public static object ParseOrDefault(string input, Type targetType, object defaultValue, IFormatProvider formatProvider)
        {
            return TryParse(input, targetType, formatProvider, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 注册自定义类型解析器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="parser">解析委托，接受字符串和格式提供器参数</param>
        /// <exception cref="ArgumentNullException">解析委托为空时抛出</exception>
        public static void RegisterParser<T>(Func<string, IFormatProvider, T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            _parsers[typeof(T)] = (s, fp) => parser(s, fp);
        }

        /// <summary>
        /// 检查是否支持指定类型的解析
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果支持解析返回true，否则返回false</returns>
        public static bool CanParse(Type type)
        {
            if (type == null)
                return false;

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            // 检查是否有自定义解析器
            if (_parsers.ContainsKey(underlyingType))
                return true;

            // 检查是否为枚举类型
            if (underlyingType.IsEnum)
                return true;

            // 检查 TypeDescriptor 是否支持
            try
            {
                return TypeDescriptor.GetConverter(underlyingType).CanConvertFrom(typeof(string));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否支持指定类型的解析（泛型版本）
        /// </summary>
        /// <typeparam name="T">要检查的类型</typeparam>
        /// <returns>如果支持解析返回true，否则返回false</returns>
        public static bool CanParse<T>()
        {
            return CanParse(typeof(T));
        }
    }
}