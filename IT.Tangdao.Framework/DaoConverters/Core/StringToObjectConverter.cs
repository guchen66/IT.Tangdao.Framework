using IT.Tangdao.Framework.Utilys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoConverters.Core
{
    public class StringToObjectConverter<T> : TypeConverter
    {
        private readonly Func<string, T> _parser;

        public StringToObjectConverter(Func<string, T> parser = null)
        {
            _parser = parser
                   ?? (Parsers.Table.TryGetValue(typeof(T), out var p)
                       ? (Func<string, T>)(s => (T)p(s))
                       : _ => throw new NotSupportedException($"未注册 {typeof(T)} 的解析器"));
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => value is string s ? _parser(s) : base.ConvertFrom(context, culture, value);
    }
}