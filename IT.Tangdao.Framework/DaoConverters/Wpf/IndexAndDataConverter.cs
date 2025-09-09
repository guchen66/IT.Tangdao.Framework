using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoConverters.Wpf
{
    public class IndexAndDataConverter : MultiValueConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] 来自 IndexExtension（序号）
            // values[1] 来自 ViewModel 的真实数据
            // values[2] 来自 Tag（或其他）
            string index = values[0] as string ?? "-1";
            object data = values[1];
            object tag = values[2];

            // 这里随便拼，也可以把整包传回 VM
            return $"{index}|{data}|{tag}";
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}