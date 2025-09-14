using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public class PlcBuilder : IPlcBuilder
    {
        public PlcBuilder(ITangdaoContainer container)
        {
            this.Container = container;
        }

        public ITangdaoContainer Container { get; }
    }
}