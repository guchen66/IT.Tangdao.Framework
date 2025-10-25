using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    // 使帮助类只在当前程序集内可见
    internal static class ConfigFolderHelper
    {
        // 个人作者名，可随意改
        private const string Author = "IT.Tangdao.Framework";

        // 产品/库名，也可硬编码
        private const string Product = "TangdaoFramework";

        // 配置根目录：%LocalAppData%\Author\Product
        private static string ConfigFolder =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Author,
                Product);

        /// <summary>
        /// 确保目录存在；若失败则吞掉异常（封闭场景下不对外抛错）
        /// </summary>
        private static void EnsureFolder()
        {
            try
            {
                if (!Directory.Exists(ConfigFolder))
                    Directory.CreateDirectory(ConfigFolder);
            }
            catch
            { /* 封闭场景：静默失败 */ }
        }

        /// <summary>
        /// 内部读取文本；文件不存在时自动写入默认值并返回默认值
        /// </summary>
        internal static string ReadText(string fileName, string defaultValue = "")
        {
            EnsureFolder();
            var fullPath = Path.Combine(ConfigFolder, fileName);
            try
            {
                if (!File.Exists(fullPath))
                {
                    WriteText(fileName, defaultValue);
                    return defaultValue;
                }
                return File.ReadAllText(fullPath);
            }
            catch
            {
                // 任何 IO 异常都回退到默认值
                return defaultValue;
            }
        }

        /// <summary>
        /// 内部写入文本；任何异常直接吞掉（封闭策略）
        /// </summary>
        internal static void WriteText(string fileName, string content)
        {
            EnsureFolder();
            var fullPath = Path.Combine(ConfigFolder, fileName);
            try
            {
                File.WriteAllText(fullPath, content);
            }
            catch
            { /* 封闭场景：静默失败 */ }
        }

        /// <summary>
        /// 内部删除文件；返回是否成功
        /// </summary>
        internal static bool Delete(string fileName)
        {
            try
            {
                var fullPath = Path.Combine(ConfigFolder, fileName);
                if (!File.Exists(fullPath)) return false;
                File.Delete(fullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 1. 取单个字符串；找不到返回 null
        /// </summary>
        internal static string GetValue(string key) =>
            ConfigurationManager.AppSettings[key];

        /// <summary>
        /// 2. 一次性读完 AppSettings 成字典
        /// </summary>
        internal static IReadOnlyDictionary<string, string> GetAll()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string k in ConfigurationManager.AppSettings)
                dict[k] = ConfigurationManager.AppSettings[k];
            return dict;
        }
    }
}