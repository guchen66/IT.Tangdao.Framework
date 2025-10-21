using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// Guard一个辅助类，将所有报错写在一起
    /// </summary>
    public static class TangdaoGuards
    {
        /// <summary>
        /// ThrowIfNull传入的params参数是可变的，全部传入object[]
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ThrowIfNull(params object[] parameters)
        {
            if (parameters.Any(item => item == null))
                throw new ArgumentNullException();
        }

        public static void ThrowIfNullOrEmpty(params string[] strings)
        {
            if (strings.Any(string.IsNullOrEmpty))
                throw new ArgumentNullException();
        }

        public static void ThrowIfNullOrEmpty(IEnumerable<string> strings)
        {
            ThrowIfNull(strings);
            ThrowIfNullOrEmpty(strings.ToArray());
        }

        public static void ThrowIfFileNotFound(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Can not found the specified file path. ", path);
        }

        public static void ThrowIfFolderNotFount(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Can not found the specified path {path}. ");
        }

        public static void ThrowIfInvalidPath(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                throw new DirectoryNotFoundException($"The specified path is not a valid file or directory.  ({path})");
        }

        public static void ThrowIfNot(bool condition)
        {
            if (!condition)
                throw new InvalidOperationException();
        }

        public static void ThrowIfNot(Func<bool> condition)
        {
            ThrowIfNull(condition);
            ThrowIfNot(condition());
        }

        /// <summary>
        /// 检查对象是否为 null，如果是，则抛出 ArgumentNullException。
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="parameter">要检查的对象</param>
        /// <param name="name">参数名称（可选）</param>
        public static void ThrowIfNull<T>(T parameter, string name = null)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// 检查集合是否为空，如果是，则抛出 ArgumentException。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="collection">要检查的集合</param>
        /// <param name="message">异常消息（可选）</param>
        public static void ThrowIfEmpty<T>(IEnumerable<T> collection, string message = null)
        {
            if (!collection.Any())
            {
                throw new ArgumentException(message ?? "Collection must not be empty.");
            }
        }

        /// <summary>
        /// 检查数字是否在指定范围内，如果不在，则抛出 ArgumentOutOfRangeException。
        /// </summary>
        /// <param name="value">要检查的数值</param>
        /// <param name="min">最小值（包含）</param>
        /// <param name="max">最大值（包含）</param>
        /// <param name="paramName">参数名称（可选）</param>
        public static void ThrowIfOutOfRange(int value, int min, int max, string paramName = null)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
            }
        }

        /// <summary>
        /// 检查日期是否在指定范围内，如果不在，则抛出 ArgumentOutOfRangeException。
        /// </summary>
        /// <param name="date">要检查的日期</param>
        /// <param name="startDate">开始日期（包含）</param>
        /// <param name="endDate">结束日期（包含）</param>
        public static void ThrowIfDateOutOfRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            if (date < startDate || date > endDate)
            {
                throw new ArgumentOutOfRangeException(nameof(date), $"Date must be between {startDate.ToShortDateString()} and {endDate.ToShortDateString()}.");
            }
        }
    }
}