using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Converters.Core
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
}