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
    public class ConnectionStateToColorConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 根据连接状态选择颜色
            if (value != null && value is string connectionState)
            {
                if (connectionState == "连接")
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
    }
}