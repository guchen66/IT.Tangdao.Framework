using IT.Tangdao.Framework.Bootstrap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IT.Tangdao.Framework.Converters.Wpf
{
    public abstract class BindingValueConverterBase : IBindHandler, IValueConverter
    {
        public void AutoBindViewModel(DependencyObject view, Type viewType)
        {
            throw new NotImplementedException();

            
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


   
}
