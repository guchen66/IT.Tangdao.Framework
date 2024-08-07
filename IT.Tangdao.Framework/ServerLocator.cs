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
        public static ITangdaoProvider Provider { get; set; }

        public static ITangdaoProvider InitContainer(ITangdaoContainer container)
        {         
            Provider = container.Builder();
            return Provider;
        }

    }
}
