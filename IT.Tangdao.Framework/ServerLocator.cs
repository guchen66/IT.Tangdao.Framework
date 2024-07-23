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
        public static ITangdaoContainer Container { get; set; }

        public ServerLocator()
        {
            
        }
        public static ITangdaoContainer InitContainer(ITangdaoContainer container)
        {
            Container=container;
            return container;
        }
    }
}
