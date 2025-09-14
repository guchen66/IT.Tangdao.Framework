using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace IT.Tangdao.Framework.Converters.Wpf
{
    public class StringToBrushConverter : NoBindingValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorHex && ColorConverter.ConvertFromString(colorHex) is Color color)
            {
                return new SolidColorBrush(color);
            }
            return Brushes.Transparent;
        }
    }
}