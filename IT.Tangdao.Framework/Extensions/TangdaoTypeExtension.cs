using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using IT.Tangdao.Framework.Reflection;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// Type类型常用扩展方法
    /// </summary>
    public static class TangdaoTypeExtension
    {
        /// <summary>
        /// 判断当前类型是否为常见的整数主键类型（int / long）。
        /// </summary>
        public static bool IsIntegerKey(this Type type)
            => type == typeof(int) || type == typeof(long);

        /// <summary>
        ///  一个类是否具有无参构造器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHasConstructor(this Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            foreach (ConstructorInfo constructor in constructors)
            {
                if (constructor.GetParameters().Length == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 一个类是否实现接口T
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool IsHasInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }

        /// <summary>
        /// 一个类是否有特性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool IsHasAttribute(this Type type, Type attributeType)
        {
            return type.IsDefined(attributeType, false);
        }

        /// <summary>
        /// 一个类是否是另一个类的子类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsHasSon(this Type type, Type baseType)
        {
            return type.IsSubclassOf(baseType);
        }

        /// <summary>
        /// 获取所有父类+接口
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TypeMetadata Parents(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            // 1. 基类
            var baseType = type.BaseType;                 // 可能 null（object）
                                                          // 2. 所有祖先接口（含重复，若需要可 Distinct）
            var allIfs = type.GetInterfaces().Distinct()             // 当前+祖先类接口
                            .OrderBy(i => i.Name)        // 给个稳定序
                            .ToList();

            return new TypeMetadata(baseType, allIfs);
        }

        /// <summary>
        /// 判断类型是否为可空类型（Nullable<T>）
        /// </summary>
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 判断类型是不是可以为null
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type t)
             => !t.IsValueType || Nullable.GetUnderlyingType(t) != null;

        /// <summary>
        /// 获取类型的友好名称（处理泛型、可空类型等）
        /// </summary>
        public static string GetFriendlyName(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var name = type.Name.Substring(0, type.Name.IndexOf('`'));
            var genericArgs = type.GetGenericArguments()
                .Select(t => t.GetFriendlyName())
                .ToArray();

            return $"{name}<{string.Join(", ", genericArgs)}>";
        }

        /// <summary>
        /// 判断类型是否为数字类型
        /// </summary>
        public static bool IsNumericType(this Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return typeCode == TypeCode.Byte ||
                   typeCode == TypeCode.SByte ||
                   typeCode == TypeCode.UInt16 ||
                   typeCode == TypeCode.UInt32 ||
                   typeCode == TypeCode.UInt64 ||
                   typeCode == TypeCode.Int16 ||
                   typeCode == TypeCode.Int32 ||
                   typeCode == TypeCode.Int64 ||
                   typeCode == TypeCode.Decimal ||
                   typeCode == TypeCode.Double ||
                   typeCode == TypeCode.Single;
        }

        /// <summary>
        /// 获取类型的所有属性，包括继承链中的属性
        /// </summary>
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type,
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                foreach (var prop in currentType.GetProperties(flags))
                {
                    yield return prop;
                }
                currentType = currentType.BaseType;
            }
        }

        /// <summary>
        /// 判断类型是否为集合类型（List, Array, IEnumerable<T>等）
        /// </summary>
        public static bool IsCollectionType(this Type type)
        {
            if (type.IsArray)
                return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return true;

            return type.GetInterfaces()
                .Any(t => t.IsGenericType &&
                         t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        /// <summary>
        /// 获取集合类型的元素类型（如果是集合的话）
        /// </summary>
        public static Type GetCollectionElementType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            var enumerableInterface = type.GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType &&
                                   t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableInterface?.GetGenericArguments()[0];
        }

        /// <summary>
        /// 判断类型是否为匿名类型
        /// </summary>
        public static bool IsAnonymousType(this Type type)
        {
            return type.Name.Contains("AnonymousType") &&
                   type.IsSealed &&
                   type.Namespace == null &&
                   type.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }

        /// <summary>
        /// 获取类型的默认值（支持值类型和引用类型）
        /// </summary>
        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 判断类型是否可以被序列化（标记了[Serializable]特性）
        /// </summary>
        public static bool IsSerializable(this Type type)
        {
            return type.IsDefined(typeof(SerializableAttribute), false);
        }

        /// <summary>
        /// 获取类型的DisplayName（来自DisplayNameAttribute）
        /// </summary>
        public static string GetDisplayName(this Type type)
        {
            var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            return displayNameAttr?.DisplayName ?? type.Name;
        }

        /// <summary>
        /// 判断类型是否为委托类型
        /// </summary>
        public static bool IsDelegateType(this Type type)
        {
            return type.IsSubclassOf(typeof(Delegate));
        }

        /// <summary>
        /// 获取类型的所有泛型约束的友好描述
        /// </summary>
        public static string GetGenericConstraintsDescription(this Type type)
        {
            if (!type.IsGenericType)
                return "非泛型类型";

            var constraints = type.GetGenericArguments()
                .Select((arg, index) => $"T{index + 1}: {GetConstraintDescription(arg)}")
                .ToArray();

            return string.Join(", ", constraints);
        }

        private static string GetConstraintDescription(Type genericArg)
        {
            var constraints = new List<string>();

            if (genericArg.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                constraints.Add("class");
            if (genericArg.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                constraints.Add("struct");
            if (genericArg.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                constraints.Add("new()");

            var interfaceConstraints = genericArg.GetGenericParameterConstraints()
                .Where(t => t.IsInterface)
                .Select(t => t.Name);

            constraints.AddRange(interfaceConstraints);

            return constraints.Count > 0 ? string.Join(", ", constraints) : "无约束";
        }

        /// <summary>
        /// 判断类型是否具有指定的方法签名
        /// </summary>
        public static bool HasMethodWithSignature(this Type type, string methodName, Type returnType, params Type[] parameterTypes)
        {
            var method = type.GetMethod(methodName, parameterTypes);
            return method != null && method.ReturnType == returnType;
        }

        /// <summary>
        /// 获取类型的所有静态方法
        /// </summary>
        public static IEnumerable<MethodInfo> GetStaticMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// 判断类型是否为元组类型
        /// </summary>
        public static bool IsTupleType(this Type type)
        {
            return type.IsGenericType && type.FullName?.StartsWith("System.Tuple") == true ||
                   type.FullName?.StartsWith("System.ValueTuple") == true;
        }

        /// <summary>
        /// 获取类型的深度继承层级
        /// </summary>
        public static int GetInheritanceDepth(this Type type)
        {
            int depth = 0;
            var currentType = type;

            while (currentType != null && currentType != typeof(object))
            {
                depth++;
                currentType = currentType.BaseType;
            }

            return depth;
        }

        /// <summary>
        /// 一键把类型转成“人类可读”的完整名字，包含泛型参数
        /// </summary>
        public static string ToReadableName(this Type t)
        {
            if (!t.IsGenericType) return t.Name;
            return $"{t.Name.Split('`')[0]}<{string.Join(", ", t.GetGenericArguments().Select(ToReadableName))}>";
        }

        /// <summary>
        /// 查询指定类型（类或接口）的所有派生类型。
        /// 派生类型包括：类的直接/间接子类、接口的实现类、接口的派生接口。
        /// </summary>
        /// <param name="baseType">基类型，必须是类或接口。</param>
        /// <param name="assembly">要扫描的程序集；为 null 时扫描当前 AppDomain 中所有已加载程序集。</param>
        /// <param name="predicate">可选的额外筛选谓词；为 null 时不筛选。</param>
        /// <param name="matchGenericDefinitions">
        /// 是否开启开放泛型匹配。为 true 时，若 <paramref name="baseType"/> 是泛型定义
        /// （如 typeof(BaseEntity&lt;&gt;)），也会返回以该泛型定义派生的类型
        /// （如 <c>UserEntity : BaseEntity&lt;int&gt;</c>）。
        /// 注意：<see cref="Type.IsAssignableFrom(Type)"/> 对开放泛型定义恒返回 false，必须开启此开关才能匹配。
        /// </param>
        /// <returns>派生类型集合（已物化，非延迟执行）。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="baseType"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="baseType"/> 既不是类也不是接口。</exception>
        public static IEnumerable<Type> GetDerivedTypes(this Type baseType, Assembly assembly = null, Func<Type, bool> predicate = null, bool matchGenericDefinitions = false)
        {
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));
            if (!baseType.IsClass && !baseType.IsInterface)
                throw new ArgumentException("baseType 必须是类或接口。", nameof(baseType));

            var candidates = GetCandidateTypes(assembly);

            return candidates
                .Where(t => t != baseType
                            && MatchesBaseType(t, baseType, matchGenericDefinitions)
                            && (predicate == null || predicate(t)))
                .ToArray();
        }

        /// <summary>
        /// 查询指定类的所有子类（包括间接子类）。
        /// </summary>
        /// <param name="baseType">父类类型，必须是类。</param>
        /// <param name="assembly">要扫描的程序集；为 null 时扫描当前 AppDomain 中所有已加载程序集。</param>
        /// <param name="includeAbstract">是否包含抽象类与静态类；默认为 false。</param>
        /// <param name="matchGenericDefinitions">是否开启开放泛型匹配，语义同 <see cref="GetDerivedTypes"/>。</param>
        /// <returns>子类类型集合（已物化）。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="baseType"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="baseType"/> 不是类时抛出；
        /// 如需查询接口的实现类型，请改用 <see cref="GetImplementingTypes"/>。</exception>
        public static IEnumerable<Type> GetDerivedClasses(this Type baseType, Assembly assembly = null, bool includeAbstract = false, bool matchGenericDefinitions = false)
        {
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));
            if (!baseType.IsClass)
                throw new ArgumentException("baseType 必须是类。如需查询接口的实现类型，请使用 GetImplementingTypes。", nameof(baseType));

            return baseType.GetDerivedTypes(
                assembly,
                t => t.IsClass
                     && (includeAbstract || !t.IsAbstract)
                     && !t.IsGenericTypeDefinition,
                matchGenericDefinitions);
        }

        /// <summary>
        /// 查询指定接口的所有实现类（包括间接实现）。
        /// </summary>
        /// <param name="interfaceType">接口类型，必须是接口。</param>
        /// <param name="assembly">要扫描的程序集；为 null 时扫描当前 AppDomain 中所有已加载程序集。</param>
        /// <param name="includeAbstract">是否包含抽象类；默认为 false。</param>
        /// <param name="includeValueTypes">是否包含结构体等值类型实现者；默认为 false（仅返回类）。</param>
        /// <param name="matchGenericDefinitions">是否开启开放泛型匹配，语义同 <see cref="GetDerivedTypes"/>。</param>
        /// <returns>实现类类型集合（已物化）。接口继承接口的派生关系请使用 <see cref="GetDerivedInterfaces"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="interfaceType"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="interfaceType"/> 不是接口。</exception>
        public static IEnumerable<Type> GetImplementingTypes(this Type interfaceType, Assembly assembly = null, bool includeAbstract = false, bool includeValueTypes = false, bool matchGenericDefinitions = false)
        {
            if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
            if (!interfaceType.IsInterface)
                throw new ArgumentException("interfaceType 必须是接口。", nameof(interfaceType));

            return interfaceType.GetDerivedTypes(
                assembly,
                t => (t.IsClass || (includeValueTypes && t.IsValueType))
                     && (includeAbstract || !t.IsAbstract)
                     && !t.IsGenericTypeDefinition,
                matchGenericDefinitions);
        }

        /// <summary>
        /// 查询指定接口的所有派生接口（子接口，包括间接派生）。
        /// </summary>
        /// <param name="interfaceType">接口类型，必须是接口。</param>
        /// <param name="assembly">要扫描的程序集；为 null 时扫描当前 AppDomain 中所有已加载程序集。</param>
        /// <param name="matchGenericDefinitions">是否开启开放泛型匹配，语义同 <see cref="GetDerivedTypes"/>。</param>
        /// <returns>派生接口类型集合（已物化）。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="interfaceType"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="interfaceType"/> 不是接口。</exception>
        public static IEnumerable<Type> GetDerivedInterfaces(this Type interfaceType, Assembly assembly = null, bool matchGenericDefinitions = false)
        {
            if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
            if (!interfaceType.IsInterface)
                throw new ArgumentException("interfaceType 必须是接口。", nameof(interfaceType));

            return interfaceType.GetDerivedTypes(
                assembly,
                t => t.IsInterface,
                matchGenericDefinitions);
        }

        /// <summary>
        /// 查询指定父类的所有子类（泛型重载，语义同 <see cref="GetDerivedClasses(Type, Assembly, bool, bool)"/>）。
        /// </summary>
        /// <typeparam name="TParent">父类类型。</typeparam>
        /// <param name="assembly">要扫描的程序集；为 null 时扫描当前 AppDomain 中所有已加载程序集。</param>
        /// <param name="includeAbstract">是否包含抽象类；默认为 false。</param>
        /// <param name="matchGenericDefinitions">是否开启开放泛型匹配。</param>
        public static IEnumerable<Type> GetDerivedClasses<TParent>(Assembly assembly = null, bool includeAbstract = false, bool matchGenericDefinitions = false) where TParent : class
        {
            return typeof(TParent).GetDerivedClasses(assembly, includeAbstract, matchGenericDefinitions);
        }

        /// <summary>
        /// 查询指定接口的所有实现类（泛型重载，语义同 <see cref="GetImplementingTypes(Type, Assembly?, bool, bool, bool)"/>）。
        /// </summary>
        /// <typeparam name="TInterface">接口类型。</typeparam>
        /// <param name="assembly">要扫描的程序集；为 null 时扫描当前 AppDomain 中所有已加载程序集。</param>
        /// <param name="includeAbstract">是否包含抽象类；默认为 false。</param>
        /// <param name="includeValueTypes">是否包含结构体等值类型实现者；默认为 false。</param>
        /// <param name="matchGenericDefinitions">是否开启开放泛型匹配。</param>
        public static IEnumerable<Type> GetImplementingTypes<TInterface>(Assembly assembly = null, bool includeAbstract = false, bool includeValueTypes = false, bool matchGenericDefinitions = false)
        {
            return typeof(TInterface).GetImplementingTypes(assembly, includeAbstract, includeValueTypes, matchGenericDefinitions);
        }

        /// <summary>
        /// 获取待扫描的类型集合；未指定程序集时扫描当前 AppDomain 中所有已加载程序集。
        /// </summary>
        private static Type[] GetCandidateTypes(Assembly assembly)
        {
            if (assembly != null)
            {
                return GetTypesSafely(assembly);
            }

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetTypesSafely)
                .ToArray();
        }

        /// <summary>
        /// 安全获取程序集中的全部类型，避免 <see cref="ReflectionTypeLoadException"/> 中断整个扫描；
        /// 部分类型（如依赖缺失的类型）加载失败时仅返回成功加载的部分。
        /// </summary>
        private static Type[] GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 调试程序集加载失败时，可查看 ex.LoaderExceptions 获取具体失败原因。
                return ex.Types.OfType<Type>().ToArray();
            }
        }
        /// <summary>
        /// 判断 <paramref name="type"/> 是否派生自 <paramref name="baseType"/>；
        /// 开启开放泛型匹配时，额外比对泛型定义。
        /// </summary>
        private static bool MatchesBaseType(Type type, Type baseType, bool matchGenericDefinitions)
        {
            if (baseType.IsAssignableFrom(type))
            {
                return true;
            }

            if (!matchGenericDefinitions || !baseType.IsGenericTypeDefinition)
            {
                return false;
            }

            if (baseType.IsInterface)
            {
                return type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType);
            }

            for (var current = type.BaseType; current != null; current = current.BaseType)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == baseType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}