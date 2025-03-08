using System.Globalization;
using System;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace IT.Tangdao.Framework.DaoConverters
{
    public class BoolToColorConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 根据连接状态选择颜色
            if (value != null && value is bool connectionState)
            {
                if (connectionState == true)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }

            // 默认情况下返回透明色
            return new SolidColorBrush(Colors.Transparent);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush solidColorBrush)
            {
                return value == solidColorBrush;
            }
            else
            {
                return null;
            }
        }
    }
}