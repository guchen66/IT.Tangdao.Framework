using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Selectors
{
    internal sealed class FileSelector
    {
        /// <summary>
        /// 获取当前路径文件类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PathKind GetPathKind(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
                return PathKind.None;

            // 目录存在就优先认目录
            return (File.GetAttributes(path) & FileAttributes.Directory) != 0
                ? PathKind.Directory
                : PathKind.File;
        }

        /// <summary>
        /// 解析当前类型属于指定枚举
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static DaoFileType DetectFromContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return DaoFileType.None;
            }

            string trimmedContent = content.Trim();

            if (trimmedContent.StartsWith("{") && trimmedContent.EndsWith("}") ||
                trimmedContent.StartsWith("[") && trimmedContent.EndsWith("]"))
            {
                return DaoFileType.Json;
            }
            else if (trimmedContent.StartsWith("<") && trimmedContent.EndsWith(">"))
            {
                if (trimmedContent.Contains("Configuration") || trimmedContent.Contains("AppSettings"))
                {
                    return DaoFileType.Config;
                }
                return DaoFileType.Xml;
            }
            // INI 文件检测
            else if (IniHelper.IsIniFormat(trimmedContent))
            {
                return DaoFileType.Ini;
            }
            else
            {
                // 如果都不匹配，可以尝试更复杂的检测或返回None
                return DaoFileType.None;
            }
        }

        public static DaoXmlType DetectXmlStructure(XDocument doc)
        {
            if (doc == null) return DaoXmlType.Empty;

            var root = doc.Root;

            // 检查是否只有XML声明没有内容
            if (root == null) return DaoXmlType.None;

            // 检查根节点是否有子元素
            if (!root.HasElements) return DaoXmlType.Empty;

            // 获取根节点的直接子元素
            var elements = root.Elements();

            // 只有一个子元素的情况
            if (elements.Count() == 1)
            {
                return DaoXmlType.Single;
            }

            // 多个子元素的情况
            return DaoXmlType.Multiple;
        }

        /// <summary>
        /// 把 XElement 节点的**同名子元素**映射到已有对象的可写属性上。
        /// 只映射 public 实例属性，且节点名必须与属性名完全一致（大小写敏感）。
        /// 类型转换失败时静默跳过，不会抛异常。
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="node">XML 节点</param>
        /// <param name="instance">**已创建**的实例，字段会被填充</param>
        public static void MapXElementToObject<T>(XElement node, T instance)
        {
            // 1. 取出所有公共可写属性
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                // 2. 按属性名去找同名子元素
                var element = node.Element(prop.Name);
                if (element == null) continue;          // 没有对应节点就跳过

                try
                {
                    // 3. 把字符串值转成属性类型，再反射赋值
                    object value = Convert.ChangeType(element.Value, prop.PropertyType);
                    prop.SetValue(instance, value);
                }
                catch
                {
                    // 4. 转换失败（如格式不对、只读属性等）直接忽略
                }
            }
        }

        public static IEnumerable<string> SelectFilesByDaoFileType(string rootDir, DaoFileType fileType, bool searchSubdirectories = true)
        {
            if (fileType == DaoFileType.None)
                return Enumerable.Empty<string>();

            string extension = GetExtensionFromFileType(fileType);
            if (string.IsNullOrEmpty(extension))
                return Enumerable.Empty<string>();

            var searchOption = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            try
            {
                return Directory.EnumerateFiles(rootDir, $"*{extension}", searchOption);
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
        }

        public static string GetExtensionFromFileType(DaoFileType fileType)
        {
            if (fileType == DaoFileType.None)
                return null;

            if (!Enum.IsDefined(typeof(DaoFileType), fileType))
                throw new ArgumentException($"无效的 DaoFileType 值: {fileType}", nameof(fileType));

            return "." + fileType.ToString().ToLowerInvariant();
        }

        private static readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

        //public static string GetXmlData(string content)
        //{
        //    // 如果缓存中有，直接返回
        //    if (_cache.TryGetValue(filePath, out var cachedData))
        //    {
        //        return cachedData;
        //    }

        //    // 否则读取文件并缓存

        //    _cache.GetOrAdd(filePath, xmlData);
        //    return xmlData;
        //}

        //public static void ClearCache(string filePath) => _cache.TryRemove(filePath);
        // public static void ClearAllCache() => _cache = new MemoryDictionaryService(); // 或者提供清理
        /// <summary>
        /// 文件导入
        /// </summary>
        public static void Import()
        {
        }

        /// <summary>
        /// 文件导出
        /// </summary>
        public static void Export()
        {
        }
    }
}