using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Infrastructure;
using IT.Tangdao.Framework.Reflection;
using IT.Tangdao.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// 特性扫描器
    /// </summary>
    public static class AttributeSelector
    {
        /// <summary>
        /// 获取特性携带体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AttributeInfo<T>[] GetAttributeInfos<T>() where T : Attribute
        {
            return AttributeUtils.GetAttributeInfos<T>();
        }

        /// <summary>
        /// 查询包含指定特性的类
        /// </summary>
        public static Type[] GetClassesWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.GetClassesWithAttribute<TAttribute>(assembly);
        }

        /// <summary>
        /// 查询包含指定特性的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MethodInfo[] GetMethodFormAttributeTypes<T>() where T : Attribute
        {
            return AttributeUtils.GetMethodFormAttributeTypes<T>();
        }

        /// <summary>
        /// 返回同时携带类型和特性实例的元组，避免调用端再次反射。
        /// </summary>
        public static Tuple<Type, T>[] GetAttributeTypesWithInstances<T>() where T : Attribute
        {
            return AttributeUtils.GetAttributeTypesWithInstances<T>();
        }

        /// <summary>
        /// 查询包含指定特性的接口
        /// </summary>
        public static Type[] GetInterfacesWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.GetInterfacesWithAttribute<TAttribute>(assembly);
        }

        /// <summary>
        /// 查询包含指定特性的枚举
        /// </summary>
        public static Type[] GetEnumsWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.GetEnumsWithAttribute<TAttribute>(assembly);
        }

        /// <summary>
        /// 查询包含指定特性方法的类型
        /// </summary>
        public static Type[] GetTypesWithMethodAttribute<TAttribute>(BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.GetTypesWithMethodAttribute<TAttribute>(bindingFlags, assembly);
        }

        /// <summary>
        /// 查询包含指定特性属性的类型
        /// </summary>
        public static Type[] GetTypesWithPropertyAttribute<TAttribute>(BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.GetTypesWithPropertyAttribute<TAttribute>(bindingFlags, assembly);
        }

        /// <summary>
        /// 查询包含指定特性的方法（返回方法信息）
        /// </summary>
        public static MethodInfo[] GetMethodsWithAttribute<TAttribute>(BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.GetMethodsWithAttribute<TAttribute>(bindingFlags, assembly);
        }

        /// <summary>
        /// 查询包含指定特性的属性（返回属性信息）
        /// </summary>
        public static PropertyInfo[] GetPropertiesWithAttribute<TAttribute>(BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.GetPropertiesWithAttribute<TAttribute>(bindingFlags, assembly);
        }

        /// <summary>
        /// 查询包含指定特性的所有类型（包括类、接口、枚举等）
        /// </summary>
        public static Type[] GetAllTypesWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.GetAllTypesWithAttribute<TAttribute>(assembly);
        }

        /// <summary>
        /// 智能查询：根据特性允许的目标自动选择查询方法
        /// </summary>
        public static object[] AgentQuery<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            return AttributeUtils.AgentQuery<TAttribute>(assembly);
        }
    }
}