using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoProviderExtension
    {
        public static T Resolve<T>(this ITangdaoProvider provider)
        {
            return (T)provider.Resolve(typeof(T));
        }
    }
}
