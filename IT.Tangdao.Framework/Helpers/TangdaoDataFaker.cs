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
        private static readonly ConcurrentDictionary<string, Action<T, object>> _propertySetters =
            new ConcurrentDictionary<string, Action<T, object>>();

        /// <summary>
        /// 通过动态委托自动生成数据
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<T> Build(int count)
        {
            var result = new List<T>();
            for (int i = 0; i < count; i++)
            {
                result.Add(CreateRandomInstance());
            }
            FakeDataHelper.ResetCounters();
            return result;
        }

        private static Action<T, object> GetOrCreateSetter(PropertyInfo property)
        {
            return _propertySetters.GetOrAdd(property.Name, key =>
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
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);// 2. 获取所有公共实例属性

            foreach (var property in properties)  // 3. 遍历每个属性
            {
                if (property.IsDefined(typeof(IgnoreAttribute), false))
                    continue;   // **直接跳过，后续任何逻辑都不执行**
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

            var fakeDataAttr = property.GetCustomAttribute<TangdaoFakeAttribute>();

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
            if (propertyType == typeof(string)) return FakeDataHelper.GenerateRandomString();
            if (propertyType == typeof(int)) return FakeDataHelper.GenerateUniqueId();
            if (propertyType == typeof(double) || propertyType == typeof(float)) return FakeDataHelper.GenerateDoubleUniqueId();
            if (propertyType == typeof(long)) return (long)FakeDataHelper.GenerateUniqueId();
            if (propertyType == typeof(decimal)) return (long)FakeDataHelper.GenerateDecimalUniqueId();
            if (propertyType == typeof(DateTime)) return FakeDataHelper.GenerateRandomDateTime();
            if (propertyType == typeof(bool)) return FakeDataHelper.GetRandomBoolean();
            if (propertyType.IsEnum) return FakeDataHelper.GetRandomEnumValue(propertyType);

            return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        }

        private static object GenerateNestedObject(Type type, int depth)
        {
            if (depth <= 0) return null;

            // 1. 先 new 空壳
            var empty = Activator.CreateInstance(type);

            // 2. 用 CloneGenerator 快速填值（深度-1）
            var fakerType = typeof(TangdaoDataFaker<>).MakeGenericType(type);
            var buildMethod = fakerType.GetMethod("Build", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int) }, null);
            var faker = Activator.CreateInstance(fakerType);
            var filled = buildMethod.Invoke(faker, new object[] { 1 });   // 生成 1 条
            return ((System.Collections.IList)filled)[0];               // 取出第一条
        }
    }
}