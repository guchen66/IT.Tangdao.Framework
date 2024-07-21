using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoConverters
{
    public class TimeFormatConverter: ValueConverterBase
    {
        public override object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            var date = (DateTime)o;    //绑定的值
            switch (type.Name)
            {
                case "String":
                    return date.ToString("yyyy-MM-dd:HH:mm:ss", culture);
                case "Brush":
                    return Brushes.Red;
                default:
                    return o;
            }
        }

        public override object ConvertBack(object o, Type type,
            object parameter, CultureInfo culture) => null;
    }
}
