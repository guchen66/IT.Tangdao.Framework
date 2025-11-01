using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Infrastructure.Configurations;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 轻量级 INI 文件帮助类（基于 Win32 API，无依赖）
    /// </summary>
    public sealed class IniHelper
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

        private readonly string _filePath;
        private readonly object _lock = new object();

        /// <summary>
        /// 初始化并指定 INI 文件路径（若不存在会自动创建）
        /// </summary>
        public IniHelper(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("INI file path is empty.", nameof(filePath));

            _filePath = Path.GetFullPath(filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
        }

        #region 基础读写

        /// <summary>
        /// 写入字符串（若 section/key 为 null 则写入空值）
        /// </summary>
        public void WriteString(string section, string key, string value)
        {
            lock (_lock)
                _ = WritePrivateProfileString(section, key, value ?? string.Empty, _filePath);
        }

        /// <summary>
        /// 读取字符串，缺失时返回 <paramref name="defaultValue"/>
        /// </summary>
        public string ReadString(string section, string key, string defaultValue = "")
        {
            const int BufLen = 1024;                 // 1K 缓冲，够用
            char[] buffer = new char[BufLen];        // 零分配只在托管堆一次

            uint len;
            lock (_lock)
                len = GetPrivateProfileString(section, key, defaultValue ?? string.Empty, buffer, (uint)buffer.Length, _filePath);

            // len 不包含末尾 '\0'
            return new string(buffer, 0, (int)len);
        }

        /// <summary>
        /// 写入 32 位有符号整数
        /// </summary>
        public void WriteInt(string section, string key, int value) =>
            WriteString(section, key, value.ToString());

        /// <summary>
        /// 读取 32 位有符号整数，缺失返回 <paramref name="defaultValue"/>
        /// </summary>
        public int ReadInt(string section, string key, int defaultValue = 0) =>
            int.TryParse(ReadString(section, key), out var v) ? v : defaultValue;

        /// <summary>
        /// 写入布尔（true/false）
        /// </summary>
        public void WriteBool(string section, string key, bool value) =>
            WriteString(section, key, value ? "true" : "false");

        /// <summary>
        /// 读取布尔，缺失返回 <paramref name="defaultValue"/>
        /// </summary>
        public bool ReadBool(string section, string key, bool defaultValue = false) =>
            bool.TryParse(ReadString(section, key), out var v) ? v : defaultValue;

        #endregion 基础读写

        #region 批量枚举

        /// <summary>
        /// 获取 INI 中所有段落名
        /// </summary>
        public IEnumerable<string> GetSectionNames()
        {
            const uint BUF_LEN = 32 * 1024; // 32k 缓冲
            var p = Marshal.AllocHGlobal((int)BUF_LEN);
            try
            {
                uint len;
                lock (_lock)
                    len = GetPrivateProfileSectionNames(p, BUF_LEN, _filePath);

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
        public IEnumerable<string> GetSection(string section)
        {
            const uint BUF_LEN = 32 * 1024;
            var p = Marshal.AllocHGlobal((int)BUF_LEN);
            try
            {
                uint len;
                lock (_lock)
                    len = GetPrivateProfileSection(section, p, BUF_LEN, _filePath);

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