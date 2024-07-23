using IT.Tangdao.Framework.DaoDtos.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class OptionsServerExtension
    {
        public static ITangdaoContainer Configure<TOptions>(this ITangdaoContainer container,Action<TOptions> options) where TOptions : class
        {
            container.Configure(options);
            return container;
        }
    }
}
