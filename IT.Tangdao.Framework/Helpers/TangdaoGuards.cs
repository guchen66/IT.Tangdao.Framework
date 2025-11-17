using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 提供参数验证和条件检查的辅助方法
    /// 此类包含一系列静态方法，用于在方法开始时验证参数和条件。
    /// </summary>
    public static class TangdaoGuards
    {
        #region Null 检查

        /// <summary>
        /// 检查单个对象是否为 null，如果是则抛出 <see cref="ArgumentNullException"/>。
        /// </summary>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="argument">要检查的对象。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="argument"/> 为 null 时抛出。</exception>
        public static void ThrowIfNull<T>([ValidatedNotNull] T argument, string paramName = null) where T : class
        {
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// 检查多个对象是否为 null，如果任何一个为 null 则抛出 <see cref="ArgumentNullException"/>。
        /// </summary>
        /// <param name="arguments">要检查的对象数组。</param>
        /// <exception cref="ArgumentNullException">当任何一个参数为 null 时抛出。</exception>
        public static void ThrowIfAnyNull(params object[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] is null)
                {
                    throw new ArgumentNullException($"arguments[{i}]");
                }
            }
        }

        /// <summary>
        /// 检查值类型不可为空的值是否为 null，如果是则抛出 <see cref="ArgumentNullException"/>。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="argument">要检查的可空值类型。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="argument"/> 为 null 时抛出。</exception>
        public static void ThrowIfNull<T>([ValidatedNotNull] T? argument, string paramName = null) where T : struct
        {
            if (!argument.HasValue)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        #endregion Null 检查

        #region 字符串检查

        /// <summary>
        /// 检查字符串是否为 null 或空字符串，如果是则抛出 <see cref="ArgumentNullException"/> 或 <see cref="ArgumentException"/>。
        /// </summary>
        /// <param name="argument">要检查的字符串。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="argument"/> 为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当 <paramref name="argument"/> 为空字符串时抛出。</exception>
        public static void ThrowIfNullOrEmpty([ValidatedNotNull] string argument, string paramName = null)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (argument.Length == 0)
            {
                throw new ArgumentException("String cannot be empty.", paramName);
            }
        }

        /// <summary>
        /// 检查字符串是否为 null、空字符串或仅包含空白字符，如果是则抛出相应的异常。
        /// </summary>
        /// <param name="argument">要检查的字符串。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="argument"/> 为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当 <paramref name="argument"/> 为空或空白字符串时抛出。</exception>
        public static void ThrowIfNullOrWhiteSpace([ValidatedNotNull] string argument, string paramName = null)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (argument.Length == 0)
            {
                throw new ArgumentException("String cannot be empty.", paramName);
            }

            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException("String cannot consist only of white-space characters.", paramName);
            }
        }

        /// <summary>
        /// 检查多个字符串是否为 null 或空字符串，如果任何一个不符合要求则抛出异常。
        /// </summary>
        /// <param name="arguments">要检查的字符串数组。</param>
        /// <exception cref="ArgumentNullException">当数组或任何字符串为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当任何字符串为空时抛出。</exception>
        public static void ThrowIfAnyNullOrEmpty(params string[] arguments)
        {
            ThrowIfNull(arguments, nameof(arguments));

            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] is null)
                {
                    throw new ArgumentNullException($"arguments[{i}]");
                }

                if (arguments[i].Length == 0)
                {
                    throw new ArgumentException($"String at index {i} cannot be empty.", $"arguments[{i}]");
                }
            }
        }

        /// <summary>
        /// 检查字符串集合中的每个元素是否为 null 或空字符串。
        /// </summary>
        /// <param name="collection">要检查的字符串集合。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当集合为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当集合中包含 null 或空字符串时抛出。</exception>
        public static void ThrowIfAnyNullOrEmpty(IEnumerable<string> collection, string paramName = null)
        {
            ThrowIfNull(collection, paramName);

            int index = 0;
            foreach (var item in collection)
            {
                if (item is null)
                {
                    throw new ArgumentException($"Collection contains null element at index {index}.", paramName);
                }

                if (item.Length == 0)
                {
                    throw new ArgumentException($"Collection contains empty string at index {index}.", paramName);
                }
                index++;
            }
        }

        #endregion 字符串检查

        #region 集合检查

        /// <summary>
        /// 检查集合是否为 null 或空集合，如果是则抛出相应的异常。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="collection">要检查的集合。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当集合为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当集合为空时抛出。</exception>
        public static void ThrowIfNullOrEmpty<T>([ValidatedNotNull] IEnumerable<T> collection, string paramName = null)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!collection.Any())
            {
                throw new ArgumentException("Collection cannot be empty.", paramName);
            }
        }

        /// <summary>
        /// 检查集合是否包含 null 元素，如果是则抛出 <see cref="ArgumentException"/>。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="collection">要检查的集合。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当集合为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当集合包含 null 元素时抛出。</exception>
        public static void ThrowIfContainsNull<T>(IEnumerable<T> collection, string paramName = null) where T : class
        {
            ThrowIfNull(collection, paramName);

            if (collection.Any(item => item is null))
            {
                throw new ArgumentException("Collection cannot contain null elements.", paramName);
            }
        }

        #endregion 集合检查

        #region 文件系统检查

        /// <summary>
        /// 检查文件是否存在，如果不存在则抛出 <see cref="FileNotFoundException"/>。
        /// </summary>
        /// <param name="filePath">要检查的文件路径。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当文件路径为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当文件路径为空时抛出。</exception>
        /// <exception cref="FileNotFoundException">当文件不存在时抛出。</exception>
        public static void ThrowIfFileNotFound([ValidatedNotNull] string filePath, string paramName = null)
        {
            ThrowIfNullOrEmpty(filePath, paramName ?? nameof(filePath));

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The specified file was not found: {filePath}", filePath);
            }
        }

        /// <summary>
        /// 检查目录是否存在，如果不存在则抛出 <see cref="DirectoryNotFoundException"/>。
        /// </summary>
        /// <param name="directoryPath">要检查的目录路径。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当目录路径为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当目录路径为空时抛出。</exception>
        /// <exception cref="DirectoryNotFoundException">当目录不存在时抛出。</exception>
        public static void ThrowIfDirectoryNotFound([ValidatedNotNull] string directoryPath, string paramName = null)
        {
            ThrowIfNullOrEmpty(directoryPath, paramName ?? nameof(directoryPath));

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The specified directory was not found: {directoryPath}");
            }
        }

        /// <summary>
        /// 检查路径是否指向有效的文件或目录，如果都不是则抛出 <see cref="FileNotFoundException"/>。
        /// </summary>
        /// <param name="path">要检查的路径。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentNullException">当路径为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当路径为空时抛出。</exception>
        /// <exception cref="FileNotFoundException">当路径不存在时抛出。</exception>
        public static void ThrowIfInvalidPath([ValidatedNotNull] string path, string paramName = null)
        {
            ThrowIfNullOrEmpty(path, paramName ?? nameof(path));

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                throw new FileNotFoundException($"The specified path is not a valid file or directory: {path}", path);
            }
        }

        #endregion 文件系统检查

        #region 条件检查

        /// <summary>
        /// 检查条件是否为 false，如果是则抛出 <see cref="InvalidOperationException"/>。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <param name="message">异常消息（可选）。</param>
        /// <exception cref="InvalidOperationException">当条件为 false 时抛出。</exception>
        public static void ThrowIfFalse(bool condition, string message = null)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message ?? "Operation is not valid due to the current state of the object.");
            }
        }

        /// <summary>
        /// 检查条件是否为 true，如果是则抛出 <see cref="InvalidOperationException"/>。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <param name="message">异常消息（可选）。</param>
        /// <exception cref="InvalidOperationException">当条件为 true 时抛出。</exception>
        public static void ThrowIfTrue(bool condition, string message = null)
        {
            if (condition)
            {
                throw new InvalidOperationException(message ?? "Operation is not valid due to the current state of the object.");
            }
        }

        /// <summary>
        /// 检查函数返回的条件是否为 false，如果是则抛出 <see cref="InvalidOperationException"/>。
        /// </summary>
        /// <param name="conditionFunc">返回要检查的条件的函数。</param>
        /// <param name="message">异常消息（可选）。</param>
        /// <exception cref="ArgumentNullException">当条件函数为 null 时抛出。</exception>
        /// <exception cref="InvalidOperationException">当条件为 false 时抛出。</exception>
        public static void ThrowIfFalse(Func<bool> conditionFunc, string message = null)
        {
            ThrowIfNull(conditionFunc, nameof(conditionFunc));
            ThrowIfFalse(conditionFunc(), message);
        }

        #endregion 条件检查

        #region 范围检查

        /// <summary>
        /// 检查数值是否在指定范围内，如果不在范围内则抛出 <see cref="ArgumentOutOfRangeException"/>。
        /// </summary>
        /// <param name="argument">要检查的数值。</param>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="max">最大值（包含）。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentOutOfRangeException">当数值不在指定范围内时抛出。</exception>
        public static void ThrowIfOutOfRange(int argument, int min, int max, string paramName = null)
        {
            if (argument < min || argument > max)
            {
                throw new ArgumentOutOfRangeException(paramName, argument, $"Value must be between {min} and {max} (inclusive).");
            }
        }

        /// <summary>
        /// 检查数值是否小于指定最小值，如果是则抛出 <see cref="ArgumentOutOfRangeException"/>。
        /// </summary>
        /// <param name="argument">要检查的数值。</param>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentOutOfRangeException">当数值小于最小值时抛出。</exception>
        public static void ThrowIfLessThan(int argument, int min, string paramName = null)
        {
            if (argument < min)
            {
                throw new ArgumentOutOfRangeException(paramName, argument, $"Value must be greater than or equal to {min}.");
            }
        }

        /// <summary>
        /// 检查数值是否大于指定最大值，如果是则抛出 <see cref="ArgumentOutOfRangeException"/>。
        /// </summary>
        /// <param name="argument">要检查的数值。</param>
        /// <param name="max">最大值（包含）。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentOutOfRangeException">当数值大于最大值时抛出。</exception>
        public static void ThrowIfGreaterThan(int argument, int max, string paramName = null)
        {
            if (argument > max)
            {
                throw new ArgumentOutOfRangeException(paramName, argument, $"Value must be less than or equal to {max}.");
            }
        }

        /// <summary>
        /// 检查日期是否在指定范围内，如果不在范围内则抛出 <see cref="ArgumentOutOfRangeException"/>。
        /// </summary>
        /// <param name="argument">要检查的日期。</param>
        /// <param name="startDate">开始日期（包含）。</param>
        /// <param name="endDate">结束日期（包含）。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <exception cref="ArgumentOutOfRangeException">当日期不在指定范围内时抛出。</exception>
        public static void ThrowIfOutOfRange(DateTime argument, DateTime startDate, DateTime endDate, string paramName = null)
        {
            if (argument < startDate || argument > endDate)
            {
                throw new ArgumentOutOfRangeException(paramName, argument, $"Date must be between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}.");
            }
        }

        #endregion 范围检查

        #region 类型检查

        /// <summary>
        /// 检查对象是否为指定类型，如果不是则抛出 <see cref="ArgumentException"/>。
        /// </summary>
        /// <param name="argument">要检查的对象。</param>
        /// <param name="paramName">参数名称（可选）。</param>
        /// <typeparam name="TExpected">期望的类型。</typeparam>
        /// <exception cref="ArgumentNullException">当对象为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当对象不是期望类型时抛出。</exception>
        public static void ThrowIfNotType<TExpected>(object argument, string paramName = null)
        {
            ThrowIfNull(argument, paramName);

            if (argument.IsNot<TExpected>())
            {
                throw new ArgumentException($"Argument must be of type {typeof(TExpected).Name}.", paramName);
            }
        }

        #endregion 类型检查
    }
}