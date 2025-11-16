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
        /// <summary>
        /// 查询包含指定特性的类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type[] GetAttributeTypes<T>() where T : Attribute
        {
            var assemble = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assemble.GetTypes().Where(x => Attribute.IsDefined(x, typeof(T))).ToArray();
        }

        /// <summary>
        /// 查询包含指定特性的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MethodInfo[] GetMethodFormAttributeTypes<T>() where T : Attribute
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var methods = assembly.GetTypes()
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
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            return asm.GetTypes()
                      .Select(t => new { Type = t, Attr = (T)Attribute.GetCustomAttribute(t, typeof(T), inherit: false) })
                      .Where(x => x.Attr != null)
                      .Select(x => Tuple.Create(x.Type, x.Attr))
                      .ToArray();
        }

        /// <summary>
        /// 获取特性携带体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AttributeInfo<T>[] GetAttributeInfos<T>() where T : Attribute
        {
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return asm.GetTypes()
                      .Select(t => new AttributeInfo<T>(t, (T)Attribute.GetCustomAttribute(t, typeof(T), inherit: false)))
                      .Where(info => info.Attribute != null)
                      .ToArray();
        }
    }
}