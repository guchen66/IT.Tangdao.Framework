using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// string类型常用的扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// string转int类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str) =>
           string.IsNullOrWhiteSpace(str) ? -1 : int.Parse(str);

        //请注意，[NotNullWhen(true)] 属性通常用于指示当方法返回 true 时，输出参数不为 null。在这个场景中，它并不适用
        /// <summary>
        /// 将字符串转为32位int类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryToInt(this string str, out int result)
        {
            return int.TryParse(str, out result);
        }

        /// <summary>
        /// 将字符串转换为 <see cref="double"/>；
        /// 空串或格式错误返回 <see cref="double.NaN"/>。
        /// </summary>
        public static double ToDouble(this string str)
        {
            return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var val) ? val : double.NaN;
        }

        /// <summary>
        /// string转double类型，失败不抛异常
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryToDouble(this string str, out double result)
        {
            return double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        /// <summary>
        /// string转bool类型，失败不抛异常
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryToBool(this string str, out bool result)
        {
            return bool.TryParse(str, out result);
        }

        /// <summary>
        /// 把仅含一个逗号的字符串按逗号切成左右两部分，返回元组。
        /// 若逗号数量 ≠ 1，抛 FormatException。
        /// </summary>
        public static Tuple<string, string> TryToTupleFromSingleComma(this string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            int first = source.IndexOf(',');
            if (first == -1 || source.IndexOf(',', first + 1) != -1)
                throw new FormatException("字符串必须且只能包含一个逗号。");
            string left = source.Substring(0, first);
            string right = source.Substring(first + 1);
            return Tuple.Create(left, right);
        }

        /// <summary>
        /// 对字符串添加大括号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TryInsertBraces(this string str)
        {
            return $"{{{str}}}";
        }

        /// <summary>
        /// 尝试对字符串添加中括号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TryInsertBrackets(this string str)
        {
            return $"[{str}]";
        }

        /// <summary>
        /// 对字符串添加小括号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TryInsertParentheses(this string str)
        {
            return $"({str})";
        }

        /// <summary> 用任意字符在两侧各包 n 个 </summary>
        public static string TryWrap(this string str, char c, int n = 1)
        {
            return str.PadLeft(str.Length + n, c).PadRight(str.Length + n * 2, c);
        }

        /// <summary>
        /// 如果字符串头尾同时出现一对大括号，则去掉它们；否则返回原串。
        /// </summary>
        public static string TryRemoveBraces(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.Length >= 2 && str[0] == '{' && str[str.Length - 1] == '}'
                ? str.Substring(1, str.Length - 2)
                : str;
        }

        /// <summary>
        /// 如果字符串头尾同时出现一对中括号，则去掉它们；否则返回原串。
        /// </summary>
        public static string TryRemoveBrackets(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.Length >= 2 && str[0] == '[' && str[str.Length - 1] == ']'
                ? str.Substring(1, str.Length - 2)
                : str;
        }

        /// <summary>
        /// 如果字符串头尾同时出现一对小括号，则去掉它们；否则返回原串。
        /// </summary>
        public static string TryRemoveParentheses(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.Length >= 2 && str[0] == '(' && str[str.Length - 1] == ')'
                ? str.Substring(1, str.Length - 2)
                : str;
        }

        /// <summary>
        /// 如果字符串头尾同时出现一对尖括号，则去掉它们；否则返回原串。
        /// </summary>
        public static string TryRemoveAngleBrackets(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.Length >= 2 && str[0] == '<' && str[str.Length - 1] == '>'
                ? str.Substring(1, str.Length - 2)
                : str;
        }

        /// <summary>
        /// 检查字符串是否包含指定的子字符串（不区分大小写）
        /// </summary>
        public static bool ContainsIgnoreCase(this string source, string value)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (value == null) throw new ArgumentNullException(nameof(value));

            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 通过路径直接创建文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string CreateFolder(this string path)
        {
            var directory = Path.GetDirectoryName(path) ?? throw new InvalidOperationException();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return path;
        }

        /// <summary>
        /// 链式调用，可调用CreateFolder方法后继续创建流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static StreamReader ToStreamReader(this string path, Encoding encoding = null)
        {
            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }
            var result = new StreamReader(path, encoding);
            return result;
        }

        /// <summary>
        /// 继续创建流做一个简单的读取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string UseStreamReadToEnd(this string path, Encoding encoding = null)
        {
            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("文件路径不存在");
            }
            string content = string.Empty;

            using (var stream = new StreamReader(path, encoding))
            {
                content = stream.ReadToEnd();
            }
            return content;
        }

        /// <summary>
        /// 打开文件并阅读
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Stream UseFileOpenRead(this string path, Encoding encoding = null)
        {
            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }
            return File.OpenRead(path);
        }

        /// <summary>
        /// 向指定文本写入内容，注意path不能是文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        /// <param name="encoding"></param>
        public static void UseFileWriteToTxt(this string path, string contents, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            // 确保目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, contents, encoding);
        }

        /// <summary>
        /// 向指定文本写入byte内容，注意path不能是文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        /// <param name="encoding"></param>
        public static void UseFileWriteByteToTxt(this string path, byte contents, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                fileStream.WriteByte(contents);
            }
        }

        /// <summary>
        /// 安全读取文件（避免文件不存在的异常）
        /// </summary>
        /// <returns>返回文件内容，如果文件不存在返回null</returns>
        public static byte[] SafeReadAllBytes(this string path)
        {
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        /// <summary>
        /// 安全读取文本（自动处理编码）
        /// </summary>
        public static string SafeReadAllText(this string path, Encoding encoding = null)
        {
            if (!File.Exists(path)) return null;
            encoding = encoding ?? Encoding.UTF8;
            return File.ReadAllText(path, encoding);
        }

        /// <summary>
        /// 继续创建文件
        /// 并且设置缓冲区
        /// </summary>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public static FileStream ToFileStream(this string localPath)
        {
            return new FileStream(localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 1024 * 1024);
        }

#if SUPPORTS_VALUETUPLE

        public static FileStream Slice(this FileStream stream, Tuple<long, long> block)
        {
            stream.Position = block.Item1;
            return stream;
        }

#else

        /// <summary>
        /// 这个方法允许你“切片”一个已存在的 FileStream 对象，即将流的位置设置到指定的偏移量（offset），
        /// 并假定接下来的特定长度（length）字节是你的操作范围。
        /// 这在处理大型文件时非常有用，
        /// 特别是当你只需要操作文件的一部分
        /// </summary>
        public static FileStream Slice(this FileStream stream, (long offset, long length) block)
        {
            stream.Position = block.offset;
            return stream;
        }

#endif

        /// <summary>
        /// 在一个段落中，获取两个指定字符串之间的文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static string GetMatch(this string text, string p1, string p2)
        {
            var rg = new Regex("(?<=(" + p1 + "))[.\\s\\S]*?(?=(" + p2 + "))",
                RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(text).Value;
        }

        /// <summary>
        /// 判空，字符串不能为null，string.Empty,""," "
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsNotNullable(this string @this)
        {
            return !string.IsNullOrEmpty(@this) && !string.IsNullOrWhiteSpace(@this);
        }

        /// <summary>
        /// 这个方法将一个字符串截断到指定的长度，并在中间添加省略占位符（默认为 "..."）
        /// 使用场景：在用户界面中显示过长的文本时进行截断显示，例如聊天应用的消息预览
        /// </summary>
        /// <param name="this"></param>
        /// <param name="limitLength"></param>
        /// <param name="omitPlaceholder"></param>
        /// <returns></returns>
        public static string TrimMiddle(this string @this, int limitLength, string omitPlaceholder = null)
        {
            const string defaultOmitPlaceholder = "...";

            if (omitPlaceholder == null) omitPlaceholder = defaultOmitPlaceholder;

            if (string.IsNullOrEmpty(@this) || @this.Length <= limitLength) return @this;

            if (limitLength == omitPlaceholder.Length) return omitPlaceholder;

            var halfLength = (limitLength - omitPlaceholder.Length) / 2;
            var bias = (limitLength - omitPlaceholder.Length) % 2;

            var firstHalfString = @this.Substring(0, halfLength + bias);
            var secondHalfString = @this.Substring(@this.Length - halfLength);

            return $"{firstHalfString}{omitPlaceholder}{secondHalfString}";
        }

        /// <summary>
        /// 常用于java或者Mysql数据库
        /// 将GetName改为get_name
        /// </summary>
        /// <param name="upperCamelCase"></param>
        /// <returns></returns>
        public static string UpperCamelCaseToDelimiterSeparated(this string upperCamelCase)
        {
            var stringBuilder = new StringBuilder(upperCamelCase.Length, upperCamelCase.Length * 2);

            for (int i = 0; i < upperCamelCase.Length; i++)
            {
                var @char = upperCamelCase[i];
                if (char.IsUpper(@char))
                {
                    if (i != 0) stringBuilder.Append('_');
                    stringBuilder.Append(char.ToLower(@char));
                }
                else
                {
                    stringBuilder.Append(@char);
                }
            }

            return stringBuilder.ToString();
        }
    }
}