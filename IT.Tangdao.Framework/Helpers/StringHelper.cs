using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// string帮助类
    /// </summary>
    public sealed class StringHelper
    {
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <summary>
        ///  null、空、空白 或 "NULL"（忽略大小写）都视为空
        /// </summary>
        public static bool IsNullOrEmptyToken(string value)
            => string.IsNullOrWhiteSpace(value) ||
               value.Equals("NULL", StringComparison.OrdinalIgnoreCase);

        private static readonly char[] _chars
        = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// 生成一个指定长度的随机字符串，RNGCryptoServiceProvider确保安全性
        /// 使用场景，生成随机密码，会话标识
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);

                var chars = new char[length];
                for (int i = 0; i < length; i++)
                    chars[i] = _chars[bytes[i] % _chars.Length];

                return new string(chars);
            }
        }

        /// <summary>
        /// 这个字段可以作为日志标识符使用
        /// </summary>
        public static string LogId => GenerateRandomString(48);
    }
}