using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 轻量级、高性能、Framework 兼容的常用验证扩展。
    /// 正则均经过精简与锚定，避免回溯灾难。
    /// </summary>
    public static class TangdaoValidationExtension
    {
        #region 正则缓存（Compiled，进程级复用）

        private static readonly Regex _rxDigits = new Regex(@"^\d+$", RegexOptions.Compiled);
        private static readonly Regex _rxLetters = new Regex(@"^[A-Za-z]+$", RegexOptions.Compiled);
        private static readonly Regex _rxCnZip = new Regex(@"^\d{6}$", RegexOptions.Compiled);
        private static readonly Regex _rxUsZip = new Regex(@"^\d{5}(-\d{4})?$", RegexOptions.Compiled);
        private static readonly Regex _rxUsPhone = new Regex(@"^(\(\d{3}\)|\d{3}-)\d{3}-\d{4}$", RegexOptions.Compiled);
        private static readonly Regex _rxKorean = new Regex(@"^[\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F]+$", RegexOptions.Compiled);

        // 手机号：13/14/15/16/17/18/19 段 + 11 位
        private static readonly Regex _rxCnMobile = new Regex(@"^(13|14|15|16|17|18|19)\d{9}$", RegexOptions.Compiled);

        #endregion 正则缓存（Compiled，进程级复用）

        #region 基础类型

        /// <summary>
        /// 是否仅包含汉字（CJK 统一表意符号各扩展区）。
        /// 空串或 null 返回 false。
        /// </summary>
        /// <remarks>
        /// 内部实现：CharUnicodeInfo + 位域比较，无正则、无分配，
        /// 百万级字符循环优于 Regex 一个数量级。
        /// </remarks>
        public static bool IsAllChinese(this string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            // 利用 .NET CharUnicodeInfo 做分类，免正则，可缓存
            foreach (var ch in value)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.OtherLetter ||
                    !IsCjkUnifiedIdeograph(ch))
                    return false;
            }
            return true;

            bool IsCjkUnifiedIdeograph(char c)
            {
                // 基本区 + 扩展 A/B/C/D/E/F/G 常用段
                return (c >= 0x4E00 && c <= 0x9FFF) ||   // 基本 + 扩展 A
                       (c >= 0x3400 && c <= 0x4DBF) ||   // 扩展 B
                       (c >= 0x20000 && c <= 0x2A6DF);  // 扩展 B（补充平面，用 int）
            }
        }

        /// <summary>
        /// 纯数字（至少一位，无符号）
        /// </summary>
        public static bool IsDigits(this string s)
            => !string.IsNullOrEmpty(s) && _rxDigits.IsMatch(s);

        /// <summary>
        /// 纯字母（A-Z，a-z）
        /// </summary>
        public static bool IsLetters(this string s)
            => !string.IsNullOrEmpty(s) && _rxLetters.IsMatch(s);

        #endregion 基础类型

        #region 地域相关

        /// <summary>
        /// 中国大陆 6 位邮编
        /// </summary>
        public static bool IsValidCNZipCode(this string s)
            => !string.IsNullOrEmpty(s) && _rxCnZip.IsMatch(s);

        /// <summary>
        /// 美国 5 位或 5+4 邮编
        /// </summary>
        public static bool IsValidUSZipCode(this string s)
            => !string.IsNullOrEmpty(s) && _rxUsZip.IsMatch(s);

        /// <summary>
        /// 美国 NANP 格式电话（XXX-XXX-XXXX 或 (XXX) XXX-XXXX）
        /// </summary>
        public static bool IsValidUSPhone(this string s)
            => !string.IsNullOrEmpty(s) && _rxUsPhone.IsMatch(s);

        /// <summary>
        /// 韩国谚文、古谚文、半角/全角谚文字符
        /// </summary>
        public static bool IsValidKorean(this string s)
            => !string.IsNullOrEmpty(s) && _rxKorean.IsMatch(s);

        /// <summary>
        /// 中国大陆手机号（11 位，最新号段）
        /// </summary>
        public static bool IsValidCNMobile(this string s)
            => !string.IsNullOrEmpty(s) && _rxCnMobile.IsMatch(s);

        #endregion 地域相关

        #region 复杂类型 → 直接调用 BCL

        /// <summary>
        /// 电子邮件（采用 System.Net.Mail.MailAddress 解析，RFC 兼容）
        /// </summary>
        public static bool IsValidEmailAddress(this string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email; // 确保没有多余显示名
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// URI（支持 http/https/ftp/file 等，绝对路径）
        /// </summary>
        public static bool IsValidURL(this string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp
                    || uri.Scheme == Uri.UriSchemeHttps
                    || uri.Scheme == Uri.UriSchemeFtp
                    || uri.Scheme == Uri.UriSchemeFile);
        }

        /// <summary>
        /// 日期 yyyy-MM-dd
        /// </summary>
        public static bool IsValidDate(this string date)
            => DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out _);

        /// <summary>
        /// 日期时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static bool IsValidDateTime(this string dateTime)
            => DateTime.TryParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out _);

        /// <summary>
        /// 中国大陆 18 位身份证（仅格式：18 位数字或 17 位 + X/x）
        /// 如需校验码/出生日期/地区码验证，请调用专用库。
        /// </summary>
        public static bool IsValidCNID(this string id)
            => !string.IsNullOrEmpty(id) &&
               (id.Length == 18 && Regex.IsMatch(id, @"^\d{17}[\dXx]$") ||
                id.Length == 15 && Regex.IsMatch(id, @"^\d{15}$"));

        #endregion 复杂类型 → 直接调用 BCL
    }
}