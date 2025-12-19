using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Infrastructure;
using IT.Tangdao.Framework.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 扫描程序集的所有特性的类
    /// </summary>
    public class TangdaoAttributeSelector
    {
        private static Assembly DefaultAssembly => Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        /// <summary>
        /// 获取特性携带体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AttributeInfo<T>[] GetAttributeInfos<T>() where T : Attribute
        {
            return DefaultAssembly.GetTypes()
                      .Select(t => new AttributeInfo<T>(t, (T)Attribute.GetCustomAttribute(t, typeof(T), inherit: false)))
                      .Where(info => info.Attribute != null)
                      .ToArray();
        }

        /// <summary>
        /// 查询包含指定特性的类
        /// </summary>
        public static Type[] GetClassesWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            return assembly.GetTypes()
                .Where(t => t.IsClass && Attribute.IsDefined(t, typeof(TAttribute)))
                .ToArray();
        }

        /// <summary>
        /// 查询包含指定特性的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MethodInfo[] GetMethodFormAttributeTypes<T>() where T : Attribute
        {
            var methods = DefaultAssembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                .Where(method => method.IsDefined(typeof(T), false))
                .ToArray();

            return methods;
        }

        /// <summary>
        /// 返回同时携带类型和特性实例的元组，避免调用端再次反射。
        /// </summary>
        public static Tuple<Type, T>[] GetAttributeTypesWithInstances<T>() where T : Attribute
        {
            return DefaultAssembly.GetTypes()
                      .Select(t => new { Type = t, Attr = (T)Attribute.GetCustomAttribute(t, typeof(T), inherit: false) })
                      .Where(x => x.Attr != null)
                      .Select(x => Tuple.Create(x.Type, x.Attr))
                      .ToArray();
        }

        /// <summary>
        /// 查询包含指定特性的接口
        /// </summary>
        public static Type[] GetInterfacesWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            return assembly.GetTypes()
                .Where(t => t.IsInterface && Attribute.IsDefined(t, typeof(TAttribute)))
                .ToArray();
        }

        /// <summary>
        /// 查询包含指定特性的枚举
        /// </summary>
        public static Type[] GetEnumsWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            return assembly.GetTypes()
                .Where(t => t.IsEnum && Attribute.IsDefined(t, typeof(TAttribute)))
                .ToArray();
        }

        // ========== 成员级别特性 ==========

        /// <summary>
        /// 查询包含指定特性方法的类型
        /// </summary>
        public static Type[] GetTypesWithMethodAttribute<TAttribute>(BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, Assembly assembly = null) where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            var attributeType = typeof(TAttribute);

            return assembly.GetTypes()
                .Where(t => t.GetMethods(bindingFlags)
                    .Any(m => Attribute.IsDefined(m, attributeType)))
                .ToArray();
        }

        /// <summary>
        /// 查询包含指定特性属性的类型
        /// </summary>
        public static Type[] GetTypesWithPropertyAttribute<TAttribute>(
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance,
            Assembly assembly = null) where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            var attributeType = typeof(TAttribute);

            return assembly.GetTypes()
                .Where(t => t.GetProperties(bindingFlags)
                    .Any(p => Attribute.IsDefined(p, attributeType)))
                .ToArray();
        }

        /// <summary>
        /// 查询包含指定特性的方法（返回方法信息）
        /// </summary>
        public static MethodInfo[] GetMethodsWithAttribute<TAttribute>(
            Assembly assembly = null,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
            where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            var attributeType = typeof(TAttribute);

            return assembly.GetTypes()
                .SelectMany(t => t.GetMethods(bindingFlags))
                .Where(m => Attribute.IsDefined(m, attributeType))
                .ToArray();
        }

        /// <summary>
        /// 查询包含指定特性的属性（返回属性信息）
        /// </summary>
        public static PropertyInfo[] GetPropertiesWithAttribute<TAttribute>(
            Assembly assembly = null,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
            where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            var attributeType = typeof(TAttribute);

            return assembly.GetTypes()
                .SelectMany(t => t.GetProperties(bindingFlags))
                .Where(p => Attribute.IsDefined(p, attributeType))
                .ToArray();
        }

        // ========== 通用查询方法 ==========

        /// <summary>
        /// 查询包含指定特性的所有类型（包括类、接口、枚举等）
        /// </summary>
        public static Type[] GetAllTypesWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            return assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(TAttribute)))
                .ToArray();
        }

        /// <summary>
        /// 智能查询：根据特性允许的目标自动选择查询方法
        /// </summary>
        public static object[] AgnetQuery<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            if (assembly == null) assembly = DefaultAssembly;
            var attributeType = typeof(TAttribute);
            var usage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

            if (usage == null)
            {
                // 如果没有定义AttributeUsage，返回所有类型
                return GetAllTypesWithAttribute<TAttribute>(assembly);
            }

            var validOn = usage.ValidOn;
            var results = new List<object>();

            // 查询类型级别
            if (validOn.HasFlag(AttributeTargets.Class) ||
                validOn.HasFlag(AttributeTargets.Interface) ||
                validOn.HasFlag(AttributeTargets.Struct) ||
                validOn.HasFlag(AttributeTargets.Enum))
            {
                results.AddRange(GetAllTypesWithAttribute<TAttribute>(assembly));
            }

            // 查询方法级别
            if (validOn.HasFlag(AttributeTargets.Method))
            {
                results.AddRange(GetMethodsWithAttribute<TAttribute>(assembly));
            }

            // 查询属性级别
            if (validOn.HasFlag(AttributeTargets.Property))
            {
                results.AddRange(GetPropertiesWithAttribute<TAttribute>(assembly));
            }

            return results.ToArray();
        }
    }
}