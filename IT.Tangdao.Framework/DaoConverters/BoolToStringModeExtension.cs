using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace IT.Tangdao.Framework.DaoConverters
{
    public class BoolToStringModeExtension : MarkupExtension
    {
        public string TrueValue { get; set; } = "True";  // 默认值
        public string FalseValue { get; set; } = "False"; // 默认值

        // 绑定的数据源属性（bool 类型）
        public Binding Binding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding == null)
                throw new ArgumentNullException(nameof(Binding), "Binding 不能为空！");

            // 创建 Binding 并设置转换器
            var converter = new BoolToStringConverter { TrueValue = TrueValue, FalseValue = FalseValue };
            Binding.Converter = converter;

            // 返回绑定对象
            return Binding.ProvideValue(serviceProvider);
        }
    }

    // 辅助转换器
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? TrueValue : FalseValue;
            return value; // 如果不是 bool，返回原值
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}