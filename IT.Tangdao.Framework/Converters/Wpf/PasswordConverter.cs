using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IT.Tangdao.Framework.Converters.Wpf
{
    public class PasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 显示时转换为星号
            if (value is string text && !string.IsNullOrEmpty(text))
                return new string('*', text.Length);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 输入时保持原始值不变
            return value?.ToString() ?? string.Empty;
        }
    }
}