using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Infrastructure;

namespace IT.Tangdao.Framework.Utilities
{
    internal static class FileUtils
    {
        #region Win32 P/Invoke

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint WritePrivateProfileString(
            string lpSection, string lpKey, string lpString, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetPrivateProfileString(string lpSection, string lpKey, string lpDefault, char[] lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetPrivateProfileSectionNames(
            IntPtr lpszReturnBuffer, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetPrivateProfileSection(
            string lpSection, IntPtr lpReturnedString, uint nSize, string lpFileName);

        #endregion Win32 P/Invoke

        private static readonly object _lock = new object();

        #region 基础读写

        /// <summary>
        /// 写入字符串（若 section/key 为 null 则写入空值）
        /// </summary>
        public static void WriteString(string section, string key, string value, string filePath)
        {
            lock (_lock)
                _ = WritePrivateProfileString(section, key, value ?? string.Empty, filePath);
        }

        /// <summary>
        /// 读取字符串，缺失时返回 <paramref name="defaultValue"/>
        /// </summary>
        public static string ReadString(string section, string key, string defaultValue, string filePath)
        {
            const int BufLen = 1024;                 // 1K 缓冲，够用
            char[] buffer = new char[BufLen];        // 零分配只在托管堆一次

            uint len;
            lock (_lock)
                len = GetPrivateProfileString(section, key, defaultValue ?? string.Empty, buffer, (uint)buffer.Length, filePath);

            // len 不包含末尾 '\0'
            return new string(buffer, 0, (int)len);
        }

        /// <summary>
        /// 写入 32 位有符号整数
        /// </summary>
        public static void WriteInt(string section, string key, int value, string filePath) =>
            WriteString(section, key, value.ToString(), filePath);

        /// <summary>
        /// 读取 32 位有符号整数，缺失返回 <paramref name="defaultValue"/>
        /// </summary>
        public static int ReadInt(string section, string key, int defaultValue, string filePath) =>
            int.TryParse(ReadString(section, key, defaultValue.ToString(), filePath), out var v) ? v : defaultValue;

        /// <summary>
        /// 写入布尔（true/false）
        /// </summary>
        public static void WriteBool(string section, string key, bool value, string filePath) =>
            WriteString(section, key, value ? "true" : "false", filePath);

        /// <summary>
        /// 读取布尔，缺失返回 <paramref name="defaultValue"/>
        /// </summary>
        public static bool ReadBool(string section, string key, bool defaultValue, string filePath) =>
            bool.TryParse(ReadString(section, key, defaultValue.ToString(), filePath), out var v) ? v : defaultValue;

        #endregion 基础读写

        #region 批量枚举

        /// <summary>
        /// 获取 INI 中所有段落名
        /// </summary>
        public static IEnumerable<string> GetSectionNames(string filePath)
        {
            const uint BUF_LEN = 32 * 1024; // 32k 缓冲
            var p = Marshal.AllocHGlobal((int)BUF_LEN);
            try
            {
                uint len;
                lock (_lock)
                    len = GetPrivateProfileSectionNames(p, BUF_LEN, filePath);

                return BufferToStringList(p, len);
            }
            finally
            {
                Marshal.FreeHGlobal(p);
            }
        }

        /// <summary>
        /// 获取指定段落下所有键值对（格式 key=value）
        /// </summary>
        public static IEnumerable<string> GetSection(string section, string filePath)
        {
            const uint BUF_LEN = 32 * 1024;
            var p = Marshal.AllocHGlobal((int)BUF_LEN);
            try
            {
                uint len;
                lock (_lock)
                    len = GetPrivateProfileSection(section, p, BUF_LEN, filePath);

                return BufferToStringList(p, len);
            }
            finally
            {
                Marshal.FreeHGlobal(p);
            }
        }

        /// <summary>
        /// 把原生双 '\0' 结束的缓冲拆成 C# 字符串列表
        /// </summary>
        private static readonly char[] NullTerminator = { '\0' };

        private static IEnumerable<string> BufferToStringList(IntPtr p, uint len)
        {
            if (len == 0) yield break;

            var raw = Marshal.PtrToStringAuto(p, (int)len) ?? string.Empty;
            foreach (var token in raw.Split(NullTerminator, StringSplitOptions.RemoveEmptyEntries))
                yield return token;
        }

        #endregion 批量枚举

        /// <summary>
        /// 检测内容是否为 INI 文件格式
        /// </summary>
        public static bool IsIniFormat(string content) => IniParser.IsIniFormat(content);

        /// <summary>
        /// 解析 INI 文件内容
        /// </summary>
        public static IniConfigCollection Parse(string iniContent) => IniParser.Parse(iniContent);

        /// <summary>
        /// 从文件路径解析 INI 文件
        /// </summary>
        public static IniConfigCollection ParseFile(string filePath) => IniParser.ParseFile(filePath);

        // 可以添加其他 INI 相关的工具方法
        public static string GetValue(IniConfigCollection configs, string section, string key, string defaultValue = "")
        {
            var sectionConfig = configs[section];
            return sectionConfig?[key] ?? defaultValue;
        }

        public static T GetValue<T>(IniConfigCollection configs, string section, string key, T defaultValue = default)
        {
            var value = GetValue(configs, section, key);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return TypeParser.TryParse<T>(value, out var result) ? result : defaultValue;
        }
    }
}