using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoConverters
{
    /// <summary>
    /// 获得枚举的描述
    /// </summary>
    public class EnumDescriptionToValueConverter : NoBindingValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            Type type = value.GetType();
            if (!type.IsEnum) return null;

            string name = Enum.GetName(type, value);
            var field = type.GetField(name);
            var descAttr = field?.GetCustomAttribute<DescriptionAttribute>();

            return descAttr?.Description;
        }
    }
}