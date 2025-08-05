using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    public static class FakeDataHelper
    {
        private static readonly Random _random = new Random();
        private static readonly HashSet<int> _usedIds = new HashSet<int>();

        // 常用数据池
        private static readonly string[] ChineseCities = { "北京", "上海", "广州", "深圳", "杭州", "成都", "武汉", "南京" };

        private static readonly string[] CommonHobbies = { "阅读", "旅行", "摄影", "烹饪", "运动", "音乐", "电影" };
        private static readonly string[] CommonNames = { "张三", "李四", "王五", "赵六", "钱七", "孙八" };
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        // 手机号正则（符合中国手机号规则）
        private const string MobilePhonePattern = "^1[3-9]\\d{9}$";

        private static readonly Regex MobilePhoneRegex = new Regex(MobilePhonePattern);

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
            string prefix = MobilePrefixes[_random.Next(MobilePrefixes.Length)];
            string suffix = _random.Next(10000000, 99999999).ToString();
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

        public static int GenerateUniqueId(int min = 1, int max = 1000)
        {
            int id;
            do
            {
                id = _random.Next(min, max);
            } while (_usedIds.Contains(id));

            _usedIds.Add(id);
            return id;
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
                return _random.Next(min, max);
            }
            // 默认返回1-1000的随机数
            return _random.Next(1, 1001);
        }

        /// <summary>
        /// 生成随机字符串（根据Length特性决定长度）
        /// </summary>
        public static string GenerateRandomString(int? length = null)
        {
            int len = length ?? 6; // 默认长度6
            return new string(Enumerable.Repeat(Chars, len)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// 生成随机日期（包含随机时分秒）
        /// </summary>
        public static DateTime GenerateRandomDateTime(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? new DateTime(1990, 1, 1);
            endDate = endDate ?? DateTime.Today;

            int range = (endDate.Value - startDate.Value).Days;
            var randomDate = startDate.Value.AddDays(_random.Next(range));

            // 添加随机时分秒
            return randomDate
                .AddHours(_random.Next(0, 24))
                .AddMinutes(_random.Next(0, 60))
                .AddSeconds(_random.Next(0, 60));
        }

        public static object GetRandomEnumValue(Type enumType, bool returnString = false)
        {
            var values = Enum.GetValues(enumType);
            var value = values.GetValue(_random.Next(values.Length));

            // 根据需求返回枚举值或字符串
            return returnString ? value.ToString() : value;
        }

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }

        public static string GetRandomChineseCity() => ChineseCities[_random.Next(ChineseCities.Length)];

        public static string GetRandomHobby() => CommonHobbies[_random.Next(CommonHobbies.Length)];

        public static string GetRandomChineseName() => CommonNames[_random.Next(CommonNames.Length)];

        public static bool GetRandomBoolean() => _random.Next(2) == 1;

        public static int GetNextAutoIncrementId() => _intIdCounter++;
    }
}