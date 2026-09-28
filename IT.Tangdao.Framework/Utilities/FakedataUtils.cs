using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Utilities;

namespace IT.Tangdao.Framework.Utilities
{
    /// <summary>
    /// 假数据基础工具：只提供"无状态的原子造值动作"。
    /// <para>
    /// 【本版核心变更】所有随机方法都改为<b>由调用方传入 <see cref="Random"/></b>，
    /// 工具类自身不再持有任何静态可变状态：原先的 <c>ThreadLocal&lt;Random&gt;</c>、
    /// 自增计数器 <c>_intIdCounter</c>、已用 ID 集合 <c>_usedIds</c> 全部移除。
    /// </para>
    /// <para>
    /// 为什么要这么做：并行生成时，静态共享的随机源与计数器本身就是竞争点；
    /// 其中自增计数器更是<b>进程级</b>的，多个调用方同时构建数据时会互相污染序号。
    /// 把随机源的所有权交还给调用方（由生成上下文按线程持有）之后，
    /// 本类退化为纯函数集合，天然线程安全，也能用固定种子在单元测试中复现结果。
    /// </para>
    /// </summary>
    internal static class FakedataUtils
    {
        private static readonly char[] _chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// 生成一个指定长度的随机字符串，使用加密级随机源。
        /// 使用场景：生成随机密码、会话标识等对随机性要求较高的地方。
        /// <para>
        /// 本方法刻意不接受外部随机源：它追求的是密码学强度，
        /// 与业务假数据用的 <see cref="Random"/> 不是同一层级的需求。
        /// </para>
        /// </summary>
        /// <param name="length">字符串长度，必须为正数</param>
        public static string CreateRandomString(int length)
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
        public static string LogId
        {
            get { return CreateRandomString(48); }
        }

        // ===== 常用数据池 =====

        private static readonly string[] ChineseCities = { "北京", "上海", "广州", "深圳", "杭州", "成都", "武汉", "南京" };

        private static readonly string[] CommonHobbies = { "阅读", "旅行", "摄影", "烹饪", "运动", "音乐", "电影" };

        /// <summary>中文常见姓名池（保留公开：外部偶尔直接取用）。</summary>
        public static readonly string[] CommonNames = { "张三", "李四", "王五", "赵六", "钱七" };

        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        // 手机号正则（符合中国手机号规则），保留作为规则说明
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
        /// 生成符合中国规则的 11 位手机号。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        public static string GenerateChineseMobileNumber(Random random)
        {
            string prefix = MobilePrefixes[random.Next(MobilePrefixes.Length)];
            string suffix = random.Next(10000000, 99999999).ToString();
            return prefix + suffix;
        }

        /// <summary>
        /// 检查字符串是否包含"手机"或"电话"。
        /// </summary>
        /// <param name="description">待检查的描述文本</param>
        public static bool IsMobilePhoneDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return false;

            return description.Contains("手机") || description.Contains("电话");
        }

        /// <summary>
        /// 生成 int 随机数。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        /// <param name="min">下界（含）</param>
        /// <param name="max">上界（不含）</param>
        public static int GenerateUniqueId(Random random, int min = 1, int max = 1000)
        {
            NormalizeRange(ref min, ref max);
            return random.Next(min, max);
        }

        /// <summary>
        /// 生成 double 随机数（整数部分 + 指定小数位）。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        /// <param name="min">整数部分下界（含）</param>
        /// <param name="max">整数部分上界（不含）</param>
        /// <param name="point">小数位数，取值 0~15</param>
        public static double GenerateDoubleUniqueId(Random random, int min = 0, int max = 1000, int point = 4)
        {
            NormalizeRange(ref min, ref max);
            point = NormalizePoint(point);

            int integerPart = random.Next(min, max);

            if (point == 0)
            {
                return integerPart;
            }

            int maxFraction = (int)Math.Pow(10, point);
            double fractionalPart = random.Next(0, maxFraction) / (double)maxFraction;
            return Math.Round(integerPart + fractionalPart, point);
        }

        /// <summary>
        /// 生成 decimal 随机数（整数部分 + 指定小数位）。
        /// <para>不经过 double 中转，避免二进制浮点误差。</para>
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        /// <param name="min">整数部分下界（含）</param>
        /// <param name="max">整数部分上界（不含）</param>
        /// <param name="point">小数位数，取值 0~15</param>
        public static decimal GenerateDecimalUniqueId(Random random, int min = 0, int max = 1000, int point = 4)
        {
            NormalizeRange(ref min, ref max);
            point = NormalizePoint(point);

            int integerPart = random.Next(min, max);

            decimal fractionalPart = 0;
            if (point > 0)
            {
                int maxFraction = (int)Math.Pow(10, point);
                fractionalPart = (decimal)random.Next(0, maxFraction) / maxFraction;
            }

            return integerPart + fractionalPart;
        }

        /// <summary>
        /// 生成指定长度的随机字符串。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        /// <param name="length">目标长度，非正数时退化为默认长度 6</param>
        public static string GenerateRandomString(Random random, int length)
        {
            int len = length <= 0 ? 6 : length;
            char[] chars = new char[len];

            for (int i = 0; i < len; i++)
            {
                chars[i] = Chars[random.Next(Chars.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// 生成随机日期（包含随机时分秒）。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        /// <param name="startDate">下界，默认 1990-01-01</param>
        /// <param name="endDate">上界，默认当天</param>
        public static DateTime GenerateRandomDateTime(Random random, DateTime? startDate = null, DateTime? endDate = null)
        {
            DateTime start = startDate ?? new DateTime(1990, 1, 1);
            DateTime end = endDate ?? DateTime.Today;

            // 区间写反时自动纠正，避免随机抽样抛异常
            if (start > end)
            {
                DateTime temp = start;
                start = end;
                end = temp;
            }

            int range = (end - start).Days;
            DateTime randomDate = start.AddDays(range <= 0 ? 0 : random.Next(range));

            // 添加随机时分秒
            return randomDate
                .AddHours(random.Next(0, 24))
                .AddMinutes(random.Next(0, 60))
                .AddSeconds(random.Next(0, 60));
        }

        /// <summary>
        /// 随机取一个枚举成员。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        /// <param name="enumType">枚举类型</param>
        /// <param name="returnString">true 返回成员名字符串，false 返回枚举值本身</param>
        public static object GetRandomEnumValue(Random random, Type enumType, bool returnString = false)
        {
            var values = Enum.GetValues(enumType);
            var value = values.GetValue(random.Next(values.Length));

            return returnString ? value.ToString() : value;
        }

        /// <summary>
        /// 随机取一个城市名。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        public static string GetRandomChineseCity(Random random)
        {
            return ChineseCities[random.Next(ChineseCities.Length)];
        }

        /// <summary>
        /// 随机取一个爱好。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        public static string GetRandomHobby(Random random)
        {
            return CommonHobbies[random.Next(CommonHobbies.Length)];
        }

        /// <summary>
        /// 随机取一个中文姓名（固定池）。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        public static string GetRandomChineseName(Random random)
        {
            return CommonNames[random.Next(CommonNames.Length)];
        }

        /// <summary>
        /// 生成随机布尔值。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        public static bool GetRandomBoolean(Random random)
        {
            return random.Next(2) == 1;
        }

        /// <summary>
        /// 按当前线程文化随机取一个姓名。
        /// <para>
        /// 【修正说明】旧实现写作
        /// <c>GetCultureSpecificNames()[_random.Next(CommonNames.Length)]</c>：
        /// 用 <c>CommonNames</c> 的长度去索引文化姓名数组，
        /// 两个数组长度一旦不一致就会下标越界。此处改为使用被访问数组自身的长度。
        /// </para>
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        public static string GetCurrentRandomChineseName(Random random)
        {
            string[] names = CultureUtils.GetCultureSpecificNames();
            return names[random.Next(names.Length)];
        }

        /// <summary>
        /// 生成符合 <c>^[a-z]{6,12}@[a-z]{3,6}\.(com|cn)$</c> 的随机邮箱。
        /// </summary>
        /// <param name="random">调用方提供的随机源</param>
        public static string CreateRandomEmail(Random random)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyz";

            Func<int, int, string> randLower = (min, max) =>
            {
                int len = random.Next(min, max + 1);
                var sb = new StringBuilder(len);
                for (int i = 0; i < len; i++)
                    sb.Append(letters[random.Next(letters.Length)]);
                return sb.ToString();
            };

            string local = randLower(6, 12);   // [a-z]{6,12}
            string domain = randLower(3, 6);   // [a-z]{3,6}
            string tld = random.Next(2) == 0 ? "com" : "cn";

            return local + "@" + domain + "." + tld;
        }

        /// <summary>
        /// 纠正区间：min 大于 max 时交换；两者相等时把上界撑开一位，
        /// 以保证 <c>Random.Next</c> 的调用始终合法。
        /// <para>
        /// 旧实现对 <c>min &gt;= max</c> 直接抛 <c>ArgumentException</c>，
        /// 一个属性上的配置笔误就会让整批数据生成失败；
        /// 这里改为静默纠正，把"配置错误"降级为"结果不精确"，而不是中断任务。
        /// </para>
        /// </summary>
        private static void NormalizeRange(ref int min, ref int max)
        {
            if (min > max)
            {
                int temp = min;
                min = max;
                max = temp;
            }

            if (min == max)
            {
                max = min + 1;
            }
        }

        /// <summary>
        /// 把小数位数限制在 0~15，超出时收敛到边界。
        /// </summary>
        private static int NormalizePoint(int point)
        {
            if (point < 0)
            {
                return 0;
            }

            if (point > 15)
            {
                return 15;
            }

            return point;
        }
    }
}