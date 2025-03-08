using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace IT.Tangdao.Framework.Extensions
{
    public static class StringExtension
    {
        public static int ToInt(this string str)
        {
            int result = 0;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }
            try
            {
                return int.Parse(str);
            }
            catch (FormatException e) when (e.Message=="")
            {
                // 可以选择返回一个特殊值，比如 -1，或者抛出一个新的异常
                return -1;
            }
        }

        //请注意，[NotNullWhen(true)] 属性通常用于指示当方法返回 true 时，输出参数不为 null。在这个场景中，它并不适用
        public static bool TryToInt(this string str, out int result)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                result = 0; // 或者你可以选择返回 -1 或其他默认值
                return false;
            }

            // 尝试转换字符串到整数
            return int.TryParse(str, out result);
        }

        public static double ToDouble(this string str)
        {
            double result = 0;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }
            try
            {
                return double.Parse(str, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                // 可以选择返回一个特殊值，比如 double.NaN 或者 double.MinValue
                return double.NaN;
            }
        }

        public static bool TryToDouble(this string str, out double result)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                result = 0; // 或者你可以选择返回 double.NaN 或其他默认值
                return false;
            }

            // 尝试转换字符串到双精度浮点数
            return double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
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
        /// 继续创建流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static StreamReader ToStreamReader(this string path,Encoding encoding=null)
        {
            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }
            var result= new StreamReader(path, encoding);
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
                // 返回null或者空字符串，取决于您的需求
                return null;
            }
            string content=string.Empty;

            using (var stream= new StreamReader(path, encoding))
            {
                content = stream.ReadToEnd();
            }
            return content;
        }

        public static Stream UseFileOpenRead(this string path, Encoding encoding = null)
        {
            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }
            return File.OpenRead(path);
        }

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

        public static void UseFileWriteByteToTxt(this string path, byte contents, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            using (var fileStream=new FileStream(path,FileMode.OpenOrCreate))
            {
                fileStream.WriteByte(contents);
            }
            
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
        // 只有当 SUPPORTS_VALUETUPLE 编译符号被定义时才包含此代码


     
        /// <summary>
        /// 生成一个指定长度的随机字符串，RNGCryptoServiceProvider确保安全性
        /// 使用场景，生成随机密码，会话标识
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string TryToRandomString(int length)
        {
            var b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            var r = new Random(BitConverter.ToInt32(b, 0));
            var ret = string.Empty;
            const string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (var i = 0; i < length; i++)
                ret += str.Substring(r.Next(0, str.Length - 1), 1);
            return ret;
        }

        /// <summary>
        /// 这个字段可以作为日志标识符使用
        /// </summary>
        public static string LogId => TryToRandomString(48);

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

        public static bool IsEmailAddress(this string @this)
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
