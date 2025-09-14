using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
    }
}