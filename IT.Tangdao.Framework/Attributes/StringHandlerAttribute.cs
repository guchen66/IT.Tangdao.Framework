using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StringHandlerAttribute : Attribute
    {
        public StringHandlerOption Options { get; set; }

        public StringHandlerAttribute(StringHandlerOption options = StringHandlerOption.None)
        {
            Options = options;
        }

        // 运行时验证方法
        public static void ValidateUsage(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<StringHandlerAttribute>();
                if (attribute != null && property.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException(
                        $"[StringHandler]特性只能用于string类型属性。属性 '{property.Name}' 的类型是 '{property.PropertyType.Name}'");
                }
            }
        }
    }
}