using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Faker;

namespace IT.Tangdao.Framework.Helpers
{
    internal static class FakeDataHelper
    {
        // 使用ThreadLocal<Random>避免多线程竞争问题，提高性能
        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        private static readonly HashSet<int> _usedIds = new HashSet<int>();

        // 常用数据池
        private static readonly string[] ChineseCities = { "北京", "上海", "广州", "深圳", "杭州", "成都", "武汉", "南京" };

        private static readonly string[] CommonHobbies = { "阅读", "旅行", "摄影", "烹饪", "运动", "音乐", "电影" };

        public static readonly string[] CommonNames = { "张三", "李四", "王五", "赵六", "钱七" };

        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        // 手机号正则（符合中国手机号规则）
        private const string MobilePhonePattern = "^1[3-9]\\d{9}$";

        // 常用手机号前缀（中国运营商号段）
        private static readonly string[] MobilePrefixes =
        {
            "130", "131", "132", "133", "134", "135", "136", "137", "138", "139",
            "150", "151", "152", "153", "155", "156", "157", "158", "159",
            "166", "170", "171", "172", "173", "175", "176", "177", "178",
            "180", "181", "182", "183", "184", "185", "186", "187", "188", "189",
            "191", "198", "199"
        };

        /// <summary>
        /// 生成符合中国规则的11位手机号
        /// </summary>
        public static string GenerateChineseMobileNumber()
        {
            string prefix = MobilePrefixes[_random.Value.Next(MobilePrefixes.Length)];
            string suffix = _random.Value.Next(10000000, 99999999).ToString();
            return prefix + suffix;
        }

        /// <summary>
        /// 检查字符串是否包含"手机"或"电话"
        /// </summary>
        public static bool IsMobilePhoneDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return false;

            return description.Contains("手机") || description.Contains("电话");
        }

        // 自增ID计数器
        private static int _intIdCounter = 1;

        public static void ResetCounters()
        {
            _intIdCounter = 1;
            _usedIds.Clear();
        }

        /// <summary>
        /// int类型使用，生成随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GenerateUniqueId(int min = 1, int max = 1000)
        {
            // 直接返回随机数，不使用HashSet检查唯一性
            // 对于大范围的随机数，重复概率极低
            return _random.Value.Next(min, max);
        }

        /// <summary>
        /// double类型使用，生成随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double GenerateDoubleUniqueId(int min = 0, int max = 1000, int point = 4)
        {
            if (min >= max)
                throw new ArgumentException("min must be less than max");

            if (point < 0 || point > 15)
                throw new ArgumentException("point must be between 0 and 15");

            // 先生成整数，再添加小数部分
            int integerPart = _random.Value.Next(min, max);

            if (point == 0)
            {
                return integerPart;
            }
            else
            {
                int maxFraction = (int)Math.Pow(10, point);
                double fractionalPart = _random.Value.Next(0, maxFraction) / (double)maxFraction;
                double id = integerPart + fractionalPart;
                return Math.Round(id, point);
            }
        }

        /// <summary>
        /// decimal类型使用，生成随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static decimal GenerateDecimalUniqueId(int min = 0, int max = 1000, int point = 4)
        {
            if (min >= max)
                throw new ArgumentException("min must be less than max");

            if (point < 0 || point > 15)
                throw new ArgumentException("point must be between 0 and 15");

            // 生成整数部分
            int integerPart = _random.Value.Next(min, max);

            // 生成小数部分
            decimal fractionalPart = 0;
            if (point > 0)
            {
                int maxFraction = (int)Math.Pow(10, point);
                fractionalPart = (decimal)_random.Value.Next(0, maxFraction) / maxFraction;
            }

            return integerPart + fractionalPart;
        }

        /// <summary>
        /// 生成唯一数字（根据Length特性决定位数）
        /// </summary>
        public static int GenerateUniqueNumber(int? length = null)
        {
            if (length.HasValue)
            {
                // 根据Length生成指定位数的数字（如Length=3 → 100-999）
                int min = (int)Math.Pow(10, length.Value - 1);
                int max = (int)Math.Pow(10, length.Value) - 1;
                return _random.Value.Next(min, max);
            }
            // 默认返回1-1000的随机数
            return _random.Value.Next(1, 1001);
        }

        /// <summary>
        /// 生成随机字符串（根据Length特性决定长度）
        /// </summary>
        public static string GenerateRandomString(int? length = null)
        {
            int len = length ?? 6; // 默认长度6
            char[] chars = new char[len];
            int charsLength = Chars.Length;
            Random random = _random.Value;

            for (int i = 0; i < len; i++)
            {
                chars[i] = Chars[random.Next(charsLength)];
            }
            return new string(chars);
        }

        /// <summary>
        /// 生成随机日期（包含随机时分秒）
        /// </summary>
        public static DateTime GenerateRandomDateTime(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? new DateTime(1990, 1, 1);
            endDate = endDate ?? DateTime.Today;

            int range = (endDate.Value - startDate.Value).Days;
            var randomDate = startDate.Value.AddDays(_random.Value.Next(range));

            // 添加随机时分秒
            return randomDate
                .AddHours(_random.Value.Next(0, 24))
                .AddMinutes(_random.Value.Next(0, 60))
                .AddSeconds(_random.Value.Next(0, 60));
        }

        public static object GetRandomEnumValue(Type enumType, bool returnString = false)
        {
            var values = Enum.GetValues(enumType);
            var value = values.GetValue(_random.Value.Next(values.Length));

            // 根据需求返回枚举值或字符串
            return returnString ? value.ToString() : value;
        }

        /// <summary>
        /// 根据模板键返回随机值（.NET Framework 版）
        /// </summary>
        public static object GetRandomTemplateValue(string template)
        {
            switch (template)
            {
                case MockTemplate.ChineseName:
                    return GetRandomChineseName();

                case MockTemplate.Mobile:
                    return GenerateChineseMobileNumber();

                case MockTemplate.City:
                    return GetRandomChineseCity();

                case MockTemplate.Date:
                    return GenerateRandomDateTime();

                case MockTemplate.Email:
                    return StringHelper.CreateRandomEmail();

                default:
                    return GenerateRandomString(6);
            }
        }

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Value.Next(values.Length));
        }

        public static string GetRandomChineseCity() => ChineseCities[_random.Value.Next(ChineseCities.Length)];

        public static string GetRandomHobby() => CommonHobbies[_random.Value.Next(CommonHobbies.Length)];

        public static string GetRandomChineseName() => CommonNames[_random.Value.Next(CommonNames.Length)];

        public static bool GetRandomBoolean() => _random.Value.Next(2) == 1;

        public static int GetAutoIncrementId() => _intIdCounter++;

        public static string CurrentRandomChineseName => GetCurrentRandomChineseName();
        public static string RandomChineseName => GetRandomChineseName();

        public static string GetCurrentRandomChineseName()
        {
            return CultureHelper.GetCultureSpecificNames()[_random.Value.Next(CommonNames.Length)];
        }
    }
}