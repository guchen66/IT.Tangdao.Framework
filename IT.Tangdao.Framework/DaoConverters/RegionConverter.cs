using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IT.Tangdao.Framework.DaoConverters
{
    public class RegionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /*if (value is string regionName)
            {
                switch (regionName)
                {
                    case RegionNames.Header:
                        return new HeaderView();

                    case RegionNames.Aside:
                        return new AsideView();

                    case RegionNames.Content:
                        return new HomeView();

                    default:
                        throw new ArgumentException($"Unknown region: {regionName}");
                }
            }*/
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}