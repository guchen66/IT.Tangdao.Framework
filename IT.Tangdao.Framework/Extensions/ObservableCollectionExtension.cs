using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return new ObservableCollection<T>(source);
        }
    }
}
