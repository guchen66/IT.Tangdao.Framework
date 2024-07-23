using IT.Tangdao.Framework.DaoAdmin;
using IT.Tangdao.Framework.DaoDtos.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class PlcBuilderExtension
    {
        public static IPlcBuilder RegisterPlcOption(this IPlcBuilder builder,Action<PlcOption> option) 
        {
            builder.Container.Configure(option);
            return builder;
        }
    }
}
