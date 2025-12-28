using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Enums;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Extensions;
using System.Linq.Expressions;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 文件的帮助类
    /// </summary>
    public class FileHelper
    {
        // 缓存类型的属性信息，避免重复反射调用
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

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
        /// 判断该路径是否是根目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsRoot(string path)
        {
            string root = Path.GetPathRoot(path);
            return path.EqualsIgnoreCase(root);
        }

        /// <summary>
        /// 获取文件后缀类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DaoFileType GetExtension(string path)
        {
            string extension = Path.GetExtension(path).TrimStart('.');
            if (string.IsNullOrEmpty(extension))
            {
                return DaoFileType.None;
            }

            // 转换为PascalCase，比如"xml" → "Xml"
            string enumName = extension.ToFirstUpperRestLower();

            return Enum.TryParse(enumName, true, out DaoFileType result)
                ? result
                : DaoFileType.None;
        }

        #region 文件的读取

        public static string ReadAllText(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("指定文件未找到。", path);
            return File.ReadAllText(path);
        }

        public static string[] ReadAllLines(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("指定文件未找到。", path);
            return File.ReadAllLines(path);
        }

        /// <summary>
        /// .NET Framework 4.6 兼容：通过 Stream 读取全部文本（同步）
        /// </summary>
        public static string ReadTextWithStream(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("指定文件未找到。", path);

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// .NET Framework 4.6 兼容：通过 Stream 异步读取全部文本（Task 模拟）
        /// </summary>
        public static Task<string> ReadTextWithStreamAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("指定文件未找到。", path);

            // Framework 4.6 没有真正的 ReadToEndAsync，用 Task.Run 把同步 IO 抛给线程池
            return Task.Run(() =>
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: false))
                using (var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                {
                    return sr.ReadToEnd();
                }
            });
        }

        #endregion 文件的读取

        /// <summary>
        /// 解析当前类型属于指定枚举
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static DaoFileType DetectFromContent(string content)
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

        // <summary>
        /// 检测文本内容是否为XML格式
        /// 采用三级检测策略：极速过滤 → 结构验证 → 内容分析
        /// </summary>
        /// <param name="content">要检测的文本内容</param>
        /// <returns>检测结果，返回Xml、Config或None</returns>
        public static DaoFileType DetectXmlContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return DaoFileType.None;
            }

            // 第一步：检测是否为有效的XML格式
            if (!XmlTemplate.IsValidXml(content))
            {
                return DaoFileType.None;
            }

            // 第三步：普通XML格式
            return DaoFileType.Xml;
        }

        /// <summary>
        /// 解析当前XML数据的结构
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static XmlStruct DetectXmlStructure(XDocument doc)
        {
            if (doc == null) return XmlStruct.Empty;

            var root = doc.Root;

            // 检查是否只有XML声明没有内容
            if (root == null) return XmlStruct.None;

            // 检查根节点是否有子元素
            if (!root.HasElements) return XmlStruct.Empty;

            // 获取根节点的直接子元素
            var elements = root.Elements();

            // 只有一个子元素的情况
            if (elements.Count() == 1)
            {
                return XmlStruct.Single;
            }

            // 多个子元素的情况
            return XmlStruct.Multiple;
        }

        // 缓存映射委托，使用表达式树替代反射调用
        private static readonly ConcurrentDictionary<Type, Action<XElement, object>> _mapDelegatesCache = new ConcurrentDictionary<Type, Action<XElement, object>>();

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
            // 1. 从缓存获取或缓存类型的属性信息，避免重复反射调用
            var properties = _propertyCache.GetOrAdd(typeof(T), type =>
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite) // 只缓存可写属性
                    .ToArray());

            // 1. 从缓存获取或生成映射委托
            //var mapDelegate = _mapDelegatesCache.GetOrAdd(typeof(T), GenerateMapDelegate);
            //mapDelegate(node, instance);
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

        /// 只映射 public 实例属性，且节点名必须与属性名完全一致（大小写敏感）。
        /// 类型转换失败时静默跳过，不会抛异常。
        /// </summary>
        /// <summary>
        /// 生成XElement到对象的映射委托
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>映射委托</returns>
        private static Action<XElement, object> GenerateMapDelegate(Type type)
        {
            // 获取所有可写公共属性
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToArray();

            // 创建参数：XElement node, object instance
            var nodeParam = Expression.Parameter(typeof(XElement), "node");
            var instanceParam = Expression.Parameter(typeof(object), "instance");

            // 将instance转换为目标类型
            var typedInstance = Expression.Convert(instanceParam, type);

            var statements = new List<Expression>();

            foreach (var prop in properties)
            {
                // 1. 调用node.Element(prop.Name)获取子元素
                var elementMethod = typeof(XElement).GetMethod("Element", new[] { typeof(XName) });
                var nameConstant = Expression.Constant(prop.Name, typeof(string));
                var nameExpression = Expression.Call(typeof(XName), "Get", null, nameConstant);
                var elementExpression = Expression.Call(nodeParam, elementMethod, nameExpression);

                // 2. 检查element是否为null
                var nullCheck = Expression.NotEqual(elementExpression, Expression.Constant(null, typeof(XElement)));

                // 3. 调用element.Value获取值
                var valueProperty = typeof(XElement).GetProperty("Value");
                var valueExpression = Expression.Property(elementExpression, valueProperty);

                // 4. 创建本地变量来存储转换后的值
                var valueVar = Expression.Variable(prop.PropertyType, "value");

                // 5. 使用Convert.ChangeType将字符串转换为属性类型
                var changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });
                var convertExpression = Expression.Assign(
                    valueVar,
                    Expression.Convert(
                        Expression.Call(
                            changeTypeMethod,
                            Expression.Convert(valueExpression, typeof(object)),
                            Expression.Constant(prop.PropertyType, typeof(Type))
                        ),
                        prop.PropertyType
                    )
                );

                // 6. 设置属性值
                var setPropertyExpression = Expression.Call(typedInstance, prop.GetSetMethod(), valueVar);

                // 7. 将转换和赋值包装在try-catch块中
                var exceptionVar = Expression.Parameter(typeof(Exception), "ex");
                var tryBlock = Expression.TryCatch(
                    Expression.Block(
                        new[] { valueVar },
                        convertExpression,
                        setPropertyExpression
                    ),
                    Expression.Catch(typeof(Exception), exceptionVar, Expression.Empty()) // 忽略任何转换异常
                );

                // 8. 将所有逻辑包装在条件判断中
                var ifBlock = Expression.IfThen(nullCheck, tryBlock);
                statements.Add(ifBlock);
            }

            // 创建并编译表达式树
            var block = Expression.Block(statements);
            var lambda = Expression.Lambda<Action<XElement, object>>(block, nodeParam, instanceParam);
            return lambda.Compile();
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

        /// <summary>
        /// 跟据文件类型返回文件.后缀
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetExtensionFromFileType(DaoFileType fileType)
        {
            if (fileType == DaoFileType.None)
                return null;

            if (!Enum.IsDefined(typeof(DaoFileType), fileType))
                throw new ArgumentException($"无效的 DaoFileType 值: {fileType}", nameof(fileType));

            return "." + fileType.ToString().ToLowerInvariant();
        }

        private static readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
    }
}