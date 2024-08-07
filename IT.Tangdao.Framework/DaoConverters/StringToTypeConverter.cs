using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.DaoConverters
{
    public class StringToTypeConverter<T> : TypeConverter
    {
        private readonly Func<string, T> _parser;

        public StringToTypeConverter(Func<string, T> parser)
        {
            _parser = parser;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return stringValue.Split(',').Select(s => _parser(s.Trim())).ToList();
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class StringToWindowTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                var main = Application.Current.MainWindow.Owner; 
                Type targetType = main.GetType(); // 这里假设你要转换的目标类型是MainView
                ConstructorInfo constructor = targetType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    return constructor.Invoke(null);
                }
            }
            throw new NotSupportedException("Cannot convert given value to target type.");
        }
    }
}
