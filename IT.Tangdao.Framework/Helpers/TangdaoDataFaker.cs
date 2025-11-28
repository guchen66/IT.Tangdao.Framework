using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 数据自动生成器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TangdaoDataFaker<T> where T : new()
    {
        //缓存属性生成setter
        private static readonly ConcurrentDictionary<string, Action<T, object>> _cachePropertySetters =
            new ConcurrentDictionary<string, Action<T, object>>();

        // 缓存属性信息，避免重复反射
        private static readonly Lazy<PropertyInfo[]> _cachedProperties = new Lazy<PropertyInfo[]>(() =>
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !p.IsDefined(typeof(IgnoreAttribute), false))
                .ToArray());

        // 缓存属性的特性信息
        private static readonly ConcurrentDictionary<string, TangdaoFakeAttribute> _cachedAttributes =
            new ConcurrentDictionary<string, TangdaoFakeAttribute>();

        // 缓存类型到生成方法的映射，避免多次if-else判断
        private static readonly Lazy<Dictionary<Type, Func<object>>> _typeGenerators = new Lazy<Dictionary<Type, Func<object>>>(() =>
            new Dictionary<Type, Func<object>>
            {
                { typeof(string), () => FakeDataHelper.GenerateRandomString() },
                { typeof(int), () => FakeDataHelper.GenerateUniqueId() },
                { typeof(double), () => FakeDataHelper.GenerateDoubleUniqueId() },
                { typeof(float), () => FakeDataHelper.GenerateDoubleUniqueId() },
                { typeof(long), () => (long)FakeDataHelper.GenerateUniqueId() },
                { typeof(decimal), () => FakeDataHelper.GenerateDecimalUniqueId() },
                { typeof(DateTime), () => FakeDataHelper.GenerateRandomDateTime() },
                { typeof(bool), () => FakeDataHelper.GetRandomBoolean() }
            });

        /// <summary>
        /// 通过动态委托自动生成数据
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<T> Build(int count)
        {
            var array = new T[count];

            Parallel.For(0, count, i =>
            {
                array[i] = CreateRandomInstance(); // 直接赋值，无需锁
            });
            FakeDataHelper.ResetCounters();
            return array.ToList();
        }

        private static Action<T, object> GetOrCreateSetter(PropertyInfo property)
        {
            return _cachePropertySetters.GetOrAdd(property.Name, key =>
            {
                if (!property.CanWrite || property.SetMethod?.IsPublic != true)
                {
                    return (instance, value) => { };
                }

                var instanceParam = Expression.Parameter(typeof(T), "instance");
                var valueParam = Expression.Parameter(typeof(object), "value");
                var convertedValue = Expression.Convert(valueParam, property.PropertyType);
                var propertyAccess = Expression.Property(instanceParam, property);
                var assign = Expression.Assign(propertyAccess, convertedValue);

                return Expression.Lambda<Action<T, object>>(assign, instanceParam, valueParam).Compile();
            });
        }

        private static T CreateRandomInstance()
        {
            var instance = new T();  // 1. 创建新实例
            var properties = _cachedProperties.Value;  // 2. 使用缓存的属性信息

            foreach (var property in properties)  // 3. 遍历每个属性
            {
                var setter = GetOrCreateSetter(property);  // 4. 获取设置器委托
                var randomValue = GenerateRandomValue(property);  // 5. 生成随机值
                setter(instance, randomValue);  // 6. 设置属性值
            }
            return instance;
        }

        private static object GenerateRandomValue(PropertyInfo property)
        {
            // 1. 优先处理ID属性 → 零装箱分支
            if (IsIdProperty(property))
            {
                return property.PropertyType == typeof(int)
                    ? HandleIdPropertyInt()      // 返回 int，直接装 object（一次装箱不可避免）
                    : (object)HandleIdPropertyLong(); // 返回 long，直接装 object
            }

            // 使用缓存的特性信息
            var fakeDataAttr = _cachedAttributes.GetOrAdd(property.Name, key =>
                property.GetCustomAttribute<TangdaoFakeAttribute>());

            // 2. 处理带有特性的属性
            if (fakeDataAttr != null)
            {
                // 2.1 最高优先级：DefaultValue
                if (!string.IsNullOrEmpty(fakeDataAttr.DefaultValue))
                {
                    //使用C#内置的类型转换
                    return Convert.ChangeType(fakeDataAttr.DefaultValue, property.PropertyType);
                }

                // 2.2 第二优先级：Length（仅字符串）不是 string → 当场放弃，继续走后面逻辑
                if (fakeDataAttr.Length > 0 && property.PropertyType == typeof(string))
                {
                    return FakeDataHelper.GenerateRandomString(fakeDataAttr.Length);
                }

                if ((property.PropertyType == typeof(float) || property.PropertyType == typeof(double)))
                {
                    return FakeDataHelper.GenerateDoubleUniqueId(fakeDataAttr.Min, fakeDataAttr.Max, fakeDataAttr.Point);
                }

                if (property.PropertyType == typeof(decimal))
                {
                    return FakeDataHelper.GenerateDecimalUniqueId(fakeDataAttr.Min, fakeDataAttr.Max, fakeDataAttr.Point);
                }

                // 2.3 第三优先级：DataType
                if (property.PropertyType.IsEnum)
                {
                    return FakeDataHelper.GetRandomEnumValue(property.PropertyType);
                }

                // 2.4 模板键（用户显式指定）
                if (!string.IsNullOrEmpty(fakeDataAttr.Template))
                    return FakeDataHelper.GetRandomTemplateValue(fakeDataAttr.Template);
            }
            // === 3. 无特性 → 自动递归生成（深度 ≤ 2）===
            if (fakeDataAttr == null)
            {
                // 3.1 嵌套类
                if (!property.PropertyType.IsValueType && !property.PropertyType.IsArray && !property.PropertyType.IsEnum &&
                    !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string) &&
                    !property.PropertyType.FullName.StartsWith("System.", StringComparison.Ordinal))
                {
                    return GenerateNestedObject(property.PropertyType, depth: 2);
                }
            }
            // 4. 默认类型处理
            return GenerateByPropertyType(property.PropertyType);
        }

        private static bool IsIdProperty(PropertyInfo property)
        {
            return property.Name.ContainsIgnoreCase("Id") && property.PropertyType.IsIntegerKey();
        }

        private static int HandleIdPropertyInt() => FakeDataHelper.GetNextAutoIncrementId();

        private static long HandleIdPropertyLong() => (long)FakeDataHelper.GetNextAutoIncrementId();

        private static object GenerateByPropertyType(Type propertyType)
        {
            // 优先检查枚举类型
            if (propertyType.IsEnum)
                return FakeDataHelper.GetRandomEnumValue(propertyType);

            // 使用缓存的生成器，避免多次if-else判断
            if (_typeGenerators.Value.TryGetValue(propertyType, out var generator))
                return generator();

            return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        }

        private static object GenerateNestedObject(Type type, int depth)
        {
            if (depth <= 0) return null;

            // 使用缓存的嵌套对象生成器，避免多次反射
            return Activator.CreateInstance(type);
        }
    }
}