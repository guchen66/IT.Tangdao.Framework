using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Utilys
{
    public static class DictToObject
    {
        /// <summary>
        /// 字典→POCO  主入口
        /// </summary>
        public static T Convert<T>(Dictionary<string, string> dict) where T : new()
        {
            if (dict == null) return default;

            T obj = new T();

            //反射拿到 T 的所有公共属性（只认属性，不认字段）。
            var props = TypeDescriptor.GetProperties(typeof(T))
                          .Cast<PropertyDescriptor>()
                          .ToList();

            //只保留字典里存在同名键的属性
            var hits = props.Where(p => dict.ContainsKey(p.Name)).OrderBy(p => p.Name).ToList();
            foreach (var p in hits)
            {
                if (!dict.TryGetValue(p.Name, out var raw)) continue; // 跳过缺失键
                p.SetValue(obj, ConvertSingleValue(raw, p.PropertyType));
            }
            return obj;
        }

        /// <summary>
        /// 单个 string → 目标类型（利用已注册的 Parsers.Table）
        /// </summary>
        private static object ConvertSingleValue(string input, Type targetType)
        {
            // 1. 本身就是 string
            if (targetType == typeof(string))
                return input;

            // 2. 全局表里有
            if (Parsers.Table.TryGetValue(targetType, out var parser))
                return parser(input);

            // 3. 类型自带 TypeConverter
            var tc = TypeDescriptor.GetConverter(targetType);
            if (tc.CanConvertFrom(typeof(string)))
                return tc.ConvertFromInvariantString(input);

            // 4. 实在不会转
            throw new NotSupportedException($"无法将字符串转换为 {targetType}");
        }
    }
}