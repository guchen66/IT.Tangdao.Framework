using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Enums;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using IT.Tangdao.Framework.Utilities;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 目录文件查询器
    /// </summary>
    public static class FileQueryable
    {
        /// <summary>
        /// 获取程序基目录（exe执行文件前一个目录）
        /// </summary>
        /// <returns></returns>
        public static string GetDomainBaseDirectory()
        {
            string domainBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            return domainBaseDirectory;
        }

        /// <summary>
        /// 获取执行exe所在目录（包含exe）
        /// </summary>
        /// <returns></returns>
        public static string GetExecuteExeDirectory()
        {
            string exeDirectory = Assembly.GetEntryAssembly().Location;
            return exeDirectory;
        }

        /// <summary>
        /// 获取当前dll所在目录
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDllDirectory()
        {
            string dllDirectory = Assembly.GetExecutingAssembly().Location;
            return dllDirectory;
        }

        /// <summary>
        /// 递归搜索指定文件，优先当前目录，其次子目录（广度优先）
        /// </summary>
        /// <param name="fileName">目标文件名（如：appsettings.json）</param>
        /// <param name="rootDir">起始搜索目录（默认当前目录）</param>
        /// <returns>找到的第一个文件完整路径，未找到时返回null</returns>
        public static string SelectDirectoryByName(string fileName, string rootDir = null)
        {
            if (rootDir == null)
            {
                rootDir = Directory.GetCurrentDirectory();
            }

            // 优先检查当前目录
            var directPath = Path.Combine(rootDir, fileName);
            if (File.Exists(directPath))
            {
                return directPath;
            }

            // 广度优先搜索子目录
            var dirsToSearch = new Queue<string>();
            dirsToSearch.Enqueue(rootDir);

            while (dirsToSearch.Count > 0)
            {
                var currentDir = dirsToSearch.Dequeue();

                try
                {
                    // 检查当前目录文件
                    var filePath = Path.Combine(currentDir, fileName);
                    if (File.Exists(filePath))
                    {
                        return filePath;
                    }

                    // 将子目录加入队列
                    foreach (var subDir in Directory.GetDirectories(currentDir))
                    {
                        dirsToSearch.Enqueue(subDir);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // 跳过无权限访问的目录
                    continue;
                }
            }

            return null; // 未找到
        }

        /// <summary>
        /// 获取指定类库，指定某个文件夹下，并且应用了某个特性的所有类
        /// </summary>
        public static Type[] GetClassSelf(string lib, string folder, Type inter = null)
        {
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentException("文件夹不存在", nameof(folder));
            }

            try
            {
                var classTypes = GetTypesInfoByLinq(lib, folder, inter);

                return classTypes.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取类失败：{ex.Message}");
                return Array.Empty<Type>(); //new Type[0]; 避免长度为0的数组分配 // 返回空数组或者抛出异常，根据实际需求进行处理
            }
        }

        private static IEnumerable<Type> GetTypesInfoByLinq(string lib, string folder, Type inter)
        {
            // 加载指定的程序集
            Assembly callingAssembly = Assembly.Load(lib);
            // 获取程序集中所有的类型
            Type[] allTypes = callingAssembly.GetTypes();
            // 筛选出位于指定文件夹下，并且应用了指定特性的所有类
            IEnumerable<Type> modelTypes = allTypes
                .Where(type => type.Namespace?.Contains(folder) == true && (inter == null || Attribute.IsDefined(type, inter)));

            return modelTypes;
        }

        /// <summary>
        /// 获取解决方案目录
        /// </summary>
        /// <returns></returns>
        public static string GetSolutionPath()
        {
            string SolutionPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            return SolutionPath;
        }

        /// <summary>
        /// 获取解决方案Name
        /// </summary>
        /// <returns></returns>
        public static string GetSolutionName()
        {
            string SolutionPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Name;
            return SolutionPath;
        }

        /// <summary>
        /// 获取主程序所在目录
        /// </summary>
        /// <returns></returns>
        public static string GetMainProgramPath()
        {
            string MainProgramPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return MainProgramPath;
        }

        /// <summary>
        /// 获取此类的当前路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetThisFilePath([CallerFilePath] string filePath = null)
        {
            return filePath;
        }

        /// <summary>
        /// 获取当前类所在源文件相对于程序入口的相对路径（兼容 .NET Framework）
        /// </summary>
        public static string GetThisFileRelativePath([CallerFilePath] string filePath = null)
        {
            string entryDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string fullPath = Path.GetFullPath(filePath);
            return GetRelativePath(entryDir, fullPath);
        }

        /// <summary>
        /// 手动实现 GetRelativePath（兼容 .NET Framework）
        /// </summary>
        private static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) return toPath; // 不同盘符直接返回全路径

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.EqualsIgnoreCase("file"))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
        }

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
            else if (FileUtils.IsIniFormat(trimmedContent))
            {
                return DaoFileType.Ini;
            }
            else
            {
                // 如果都不匹配，可以尝试更复杂的检测或返回None
                return DaoFileType.None;
            }
        }

        /// <summary>
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
            if (!XmlContentUtils.IsValidXml(content))
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

        private static readonly ConcurrentDictionary<Type, PropertyDescriptorCollection> _propertyDescriptorCache = new ConcurrentDictionary<Type, PropertyDescriptorCollection>();

        /// <summary>
        /// 把 XElement 节点的**同名子元素**映射到已有对象的可写属性上。
        /// 只映射 public 实例属性，且节点名必须与属性名完全一致（大小写敏感）。
        /// 类型转换失败时静默跳过，不会抛异常。
        /// TypeDescriptor 把“反射”转成“查表 + 委托”，一次缓存永久复用。
        /// TypeConverter 把“通用转换”转成“专用高效解析”，无装箱拆箱。
        /// PropertyDescriptor 把“反射赋值”转成“委托调用”，JIT 可内联。
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="node">XML 节点</param>
        /// <param name="instance">**已创建**的实例，字段会被填充</param>
        public static void MapXElementToObject<T>(XElement node, T instance)
        {
            var properties = _propertyDescriptorCache.GetOrAdd(
                typeof(T),
                type => TypeDescriptor.GetProperties(type));

            foreach (PropertyDescriptor prop in properties)
            {
                if (!prop.IsReadOnly)
                {
                    var element = node.Element(prop.Name);
                    if (element != null)
                    {
                        try
                        {
                            // TypeDescriptor可以进行类型转换
                            var value = prop.Converter.ConvertFromString(element.Value);
                            prop.SetValue(instance, value);
                        }
                        catch
                        {
                            // 忽略转换错误
                        }
                    }
                }
            }
        }

        private static Action<XElement, object> CreateMapper<T>()
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToArray();

            if (properties.Length == 0)
                return (node, instance) => { }; // 空委托

            // 为每个属性创建赋值表达式
            var parameterNode = Expression.Parameter(typeof(XElement), "node");
            var parameterInstance = Expression.Parameter(typeof(object), "instance");
            var typedInstance = Expression.Convert(parameterInstance, type);

            var expressions = new List<Expression>();

            foreach (var prop in properties)
            {
                // 获取元素值：node.Element(prop.Name)?.Value
                var elementCall = Expression.Call(
                    parameterNode,
                    typeof(XElement).GetMethod("Element", new[] { typeof(string) }),
                    Expression.Constant(prop.Name));

                var valueProperty = Expression.Property(elementCall, nameof(XElement.Value));
                var elementHasValue = Expression.NotEqual(elementCall, Expression.Constant(null));
                var elementValue = Expression.Condition(
                    elementHasValue,
                    valueProperty,
                    Expression.Constant(string.Empty));

                // 转换为目标类型
                var changeTypeCall = Expression.Call(
                    typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) }),
                    elementValue,
                    Expression.Constant(prop.PropertyType));

                var convertedValue = Expression.Convert(changeTypeCall, prop.PropertyType);

                // 设置属性值
                var setProperty = Expression.Assign(
                    Expression.Property(typedInstance, prop),
                    convertedValue);

                expressions.Add(setProperty);
            }

            var body = Expression.Block(expressions);
            var lambda = Expression.Lambda<Action<XElement, object>>(body, parameterNode, parameterInstance);

            return lambda.Compile(); // 编译成委托，执行速度快
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