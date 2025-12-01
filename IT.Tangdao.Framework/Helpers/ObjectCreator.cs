using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 对象创建工具类，提供高效的对象实例化、属性映射等功能
    /// </summary>
    public static class ObjectCreator
    {
        /// <summary>
        /// 缓存无参构造函数的委托，避免重复生成表达式树
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Delegate> _constructorCache = new ConcurrentDictionary<Type, Delegate>();

        /// <summary>
        /// 缓存属性映射函数，避免重复生成表达式树
        /// </summary>
        private static readonly ConcurrentDictionary<(Type, Type), Delegate> _mapCache = new ConcurrentDictionary<(Type, Type), Delegate>();

        /// <summary>
        /// 创建无参构造函数的对象实例
        /// 优化点：使用表达式树生成构造函数委托，缓存结果，避免重复反射
        /// </summary>
        /// <typeparam name="TType">要创建的对象类型</typeparam>
        /// <returns>创建的对象实例</returns>
        public static TType CreateInstance<TType>() where TType : class
        {
            // 尝试从缓存获取构造函数委托
            if (!_constructorCache.TryGetValue(typeof(TType), out var constructor))
            {
                // 生成构造函数表达式树
                var newExpr = Expression.New(typeof(TType));
                constructor = Expression.Lambda<Func<TType>>(newExpr).Compile();

                // 缓存构造函数委托
                _constructorCache.TryAdd(typeof(TType), constructor);
            }

            // 调用构造函数委托创建实例
            return ((Func<TType>)constructor)();
        }

        /// <summary>
        /// 创建带参数构造函数的对象实例
        /// 注意：带参数构造函数暂未实现表达式树优化，仍使用Activator
        /// </summary>
        /// <typeparam name="TType">要创建的对象类型</typeparam>
        /// <param name="args">构造函数参数</param>
        /// <returns>创建的对象实例</returns>
        public static TType CreateInstance<TType>(params object[] args) where TType : class
        {
            if (args == null || args.Length == 0)
                return CreateInstance<TType>();

            return (TType)Activator.CreateInstance(typeof(TType), args);
        }

        /// <summary>
        /// 通过类型名称创建对象实例（适用于延迟加载）
        /// </summary>
        /// <typeparam name="TType">要创建的对象类型</typeparam>
        /// <param name="typeName">类型的完全限定名称</param>
        /// <returns>创建的对象实例</returns>
        /// <exception cref="ArgumentNullException">当typeName为null或空字符串时抛出</exception>
        /// <exception cref="TypeLoadException">当无法找到指定类型时抛出</exception>
        public static TType CreateInstanceFromName<TType>(string typeName) where TType : class
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName));

            Type type = Type.GetType(typeName);
            if (type == null)
                throw new TypeLoadException($"无法找到类型: {typeName}");

            return (TType)Activator.CreateInstance(type);
        }

        /// <summary>
        /// 运行时拷贝对象属性，生成高效的属性映射函数
        /// 优化点：缓存映射函数，避免重复生成表达式树
        /// </summary>
        /// <typeparam name="TSource">源对象类型</typeparam>
        /// <typeparam name="TTarget">目标对象类型</typeparam>
        /// <returns>属性映射函数，接收源对象返回目标对象</returns>
        public static Func<TSource, TTarget> CreateMap<TSource, TTarget>()
        {
            var key = (typeof(TSource), typeof(TTarget));

            // 尝试从缓存获取映射函数
            if (!_mapCache.TryGetValue(key, out var map))
            {
                // 生成属性映射表达式树
                var parameter = Expression.Parameter(typeof(TSource), "source");

                // 获取目标类型的所有公共属性
                var targetProperties = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite);

                // 获取源类型的所有公共属性
                var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

                // 生成属性绑定表达式
                var bindings = new List<MemberBinding>();
                foreach (var targetProp in targetProperties)
                {
                    // 查找源类型中名称匹配的属性
                    if (sourceProperties.TryGetValue(targetProp.Name, out var sourceProp))
                    {
                        // 检查属性类型是否兼容
                        if (targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                        {
                            // 生成属性绑定：target.Property = source.Property
                            var sourceAccess = Expression.Property(parameter, sourceProp);
                            bindings.Add(Expression.Bind(targetProp, sourceAccess));
                        }
                    }
                }

                // 生成对象初始化表达式
                var newExpr = Expression.New(typeof(TTarget));
                var memberInit = Expression.MemberInit(newExpr, bindings);

                // 编译为委托并缓存
                map = Expression.Lambda<Func<TSource, TTarget>>(memberInit, parameter).Compile();
                _mapCache.TryAdd(key, map);
            }

            // 返回缓存的映射函数
            return (Func<TSource, TTarget>)map;
        }

        /// <summary>
        /// 尝试创建对象实例，失败返回null而不是抛出异常
        /// 优化点：内部调用优化后的CreateInstance方法
        /// </summary>
        /// <typeparam name="TType">要创建的对象类型</typeparam>
        /// <returns>创建的对象实例，失败返回null</returns>
        public static TType TryCreateInstance<TType>() where TType : class
        {
            try
            {
                return CreateInstance<TType>();
            }
            catch
            {
                return null;
            }
        }
    }
}