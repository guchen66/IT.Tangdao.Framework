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
    public class TangdaoFakeDataGenerator<T> where T : new()
    {
        private static readonly ConcurrentDictionary<string, Action<T, object>> _propertySetters =
            new ConcurrentDictionary<string, Action<T, object>>();

        /// <summary>
        /// 通过动态委托自动生成数据
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<T> GenerateRandomData(int count)
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
                var setter = GetOrCreateSetter(property);  // 4. 获取设置器委托
                var randomValue = GenerateRandomValue(property);  // 5. 生成随机值
                setter(instance, randomValue);  // 6. 设置属性值
            }
            return instance;
        }

        private static object GenerateRandomValue(PropertyInfo property)
        {
            // 1. 优先处理ID属性
            // 1. 优先处理ID属性 → 零装箱分支
            if (IsIdProperty(property))
            {
                return property.PropertyType == typeof(int)
                    ? HandleIdPropertyInt()      // 返回 int，直接装 object（一次装箱不可避免）
                    : (object)HandleIdPropertyLong(); // 返回 long，直接装 object
            }

            var fakeDataAttr = property.GetCustomAttribute<FakeDataInfoAttribute>();

            // 2. 处理带有特性的属性
            if (fakeDataAttr != null)
            {
                // 2.1 最高优先级：DefaultValue
                if (!string.IsNullOrEmpty(fakeDataAttr.DefaultValue))
                {
                    //使用C#内置的类型转换
                    return Convert.ChangeType(fakeDataAttr.DefaultValue, property.PropertyType);
                }

                // 2.2 第二优先级：Length
                if (fakeDataAttr.Length > 0 && IsLengthSupportedType(property.PropertyType))
                {
                    return GenerateByLength(
                        propertyType: property.PropertyType,
                        length: fakeDataAttr.Length
                    );
                }
                // 2.3 第三优先级：DataType

                if (fakeDataAttr.DataType != null)
                {
                    if (fakeDataAttr.DataType != null)
                    {
                        // 专用于枚举/特殊类型生成
                        return GenerateByDataType(dataType: fakeDataAttr.DataType, targetPropertyType: property.PropertyType);
                    }
                }

                // 2.5 只有Description时
                if (!string.IsNullOrEmpty(fakeDataAttr.Description))
                {
                    return GenerateByDescription(fakeDataAttr.Description, fakeDataAttr.Length);
                }
            }

            // 3. 默认类型处理
            return GenerateByPropertyType(property.PropertyType);
        }

        private static bool IsIdProperty(PropertyInfo property)
        {
            return property.Name.ContainsIgnoreCase("Id") && property.PropertyType.IsIntegerKey();
        }

        private static int HandleIdPropertyInt() => FakeDataHelper.GetNextAutoIncrementId();

        private static long HandleIdPropertyLong() => (long)FakeDataHelper.GetNextAutoIncrementId();

        private static object ApplyDescriptionModifier(object baseValue, string description)
        {
            // 这里实现你的修饰逻辑，例如：
            // 如果是字符串类型，可以添加前缀/后缀
            if (baseValue is string strValue)
            {
                return $"{description}:{strValue}";
            }
            return baseValue;
        }

        private static object GenerateByDataType(Type dataType, Type targetPropertyType)
        {
            if (dataType.IsEnum)
            {
                // 如果目标属性是string，则返回枚举的字符串表示
                bool returnString = targetPropertyType == typeof(string);
                return FakeDataHelper.GetRandomEnumValue(dataType, returnString);
            }

            // 其他数据类型处理...
            throw new NotSupportedException($"不支持的数据类型: {dataType.Name}");
        }

        private static string GenerateByDescription(string description, int? length)
        {
            // 这里可以根据description返回特定类型的随机数据
            // 检查是否是手机号描述
            if (FakeDataHelper.IsMobilePhoneDescription(description))
            {
                return FakeDataHelper.GenerateChineseMobileNumber();
            }
            switch (description.ToLower())
            {
                case "姓名": return FakeDataHelper.GetRandomChineseName();
                case "城市": return FakeDataHelper.GetRandomChineseCity();
                case "爱好": return FakeDataHelper.GetRandomHobby();

                default:
                    return length.HasValue ?
                    FakeDataHelper.GenerateRandomString(length.Value) :
                    description;
            }
        }

        // 判断属性类型是否支持Length控制
        private static bool IsLengthSupportedType(Type type) =>
            type == typeof(int) || type == typeof(long) || type == typeof(string);

        private static object GenerateByLength(Type propertyType, int length)
        {
            if (propertyType == typeof(string))
                return FakeDataHelper.GenerateRandomString(length);

            if (propertyType == typeof(int) || propertyType == typeof(long))
                return FakeDataHelper.GenerateUniqueNumber(length);

            throw new NotSupportedException($"Length not supported for type {propertyType.Name}");
        }

        private static object GenerateByPropertyType(Type propertyType)
        {
            if (propertyType == typeof(string)) return FakeDataHelper.GenerateRandomString();
            if (propertyType == typeof(int)) return FakeDataHelper.GenerateUniqueId();
            if (propertyType == typeof(long)) return (long)FakeDataHelper.GenerateUniqueId();
            if (propertyType == typeof(DateTime)) return FakeDataHelper.GenerateRandomDateTime();
            if (propertyType == typeof(bool)) return FakeDataHelper.GetRandomBoolean();
            if (propertyType.IsEnum) return FakeDataHelper.GetRandomEnumValue(propertyType);

            return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        }
    }
}