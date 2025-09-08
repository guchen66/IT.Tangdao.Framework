using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace IT.Tangdao.Framework.DaoConverters
{
    public class StringToImageRelativeConverter : NoBindingValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;
            if (!string.IsNullOrEmpty(path))
            {
                return new BitmapImage(new Uri(path, UriKind.Relative)) { CacheOption = BitmapCacheOption.OnLoad };
            }
            else
            {
                return null;
            }
        }
    }
}