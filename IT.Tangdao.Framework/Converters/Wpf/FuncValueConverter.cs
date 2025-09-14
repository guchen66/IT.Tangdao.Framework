using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.Converters.Wpf
{
    public sealed class FuncValueConverter<TIn, TOut> : ValueConverterBase
    {
        private readonly Func<TIn, TOut> _convert;
        private readonly Func<TOut, TIn> _convertBack;

        public FuncValueConverter(Func<TIn, TOut> convert, Func<TOut, TIn> convertBack = null)
        {
            _convert = convert;
            _convertBack = convertBack;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return DependencyProperty.UnsetValue;
            }

            var typeConverter = TypeDescriptor.GetConverter(typeof(TIn));
            if (!(value is TIn obj))
            {
                if (typeConverter.CanConvertFrom(value.GetType()))
                {
                    obj = (TIn)typeConverter.ConvertFrom(value);
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            return _convert(obj);
        }
    }
}