using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public class ServerLocator
    {
        public static ITangdaoProvider Current { get; set; }

        public static ITangdaoProvider InitContainer(ITangdaoContainer container)
        {
            Current = container.Builder();
            return Current;
        }

        public static T Get<T>() => Current.Resolve<T>();
    }

    public class ServerLocator<TSource> : ServerLocator
    {
        public TSource TSources => Current.Resolve<TSource>();
    }
}