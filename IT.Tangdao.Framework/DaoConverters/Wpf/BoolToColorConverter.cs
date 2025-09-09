using System.Globalization;
using System;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace IT.Tangdao.Framework.DaoConverters.Wpf
{
    public class BoolToColorConverter : DependencyObject, IValueConverter
    {
        // 定义两个依赖属性，用于 XAML 绑定
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register(
                "TrueValue",
                typeof(SolidColorBrush),
                typeof(BoolToColorConverter),
                new PropertyMetadata(new SolidColorBrush(Colors.Green))); // 默认 true=绿色

        //var defaultTextColor = SystemColors.WindowTextBrush; // 返回当前系统的默认文本颜色
        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register(
                "FalseValue",
                typeof(SolidColorBrush),
                typeof(BoolToColorConverter),
                new PropertyMetadata(new SolidColorBrush(Colors.Red))); // 默认 false=红色

        // TrueValue 属性（XAML 可设置）
        public SolidColorBrush TrueValue
        {
            get => (SolidColorBrush)GetValue(TrueValueProperty);
            set => SetValue(TrueValueProperty, value);
        }

        // FalseValue 属性（XAML 可设置）
        public SolidColorBrush FalseValue
        {
            get => (SolidColorBrush)GetValue(FalseValueProperty);
            set => SetValue(FalseValueProperty, value);
        }

        // 转换逻辑
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueValue : FalseValue;
            }
            return new SolidColorBrush(Colors.Transparent); // 默认透明
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}