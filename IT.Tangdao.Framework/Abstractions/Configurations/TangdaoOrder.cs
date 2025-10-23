using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Configurations
{
    internal class TangdaoOrder : ITangdaoOrder
    {
        public int Value { get; set; }

        public TangdaoOrder(int value)
        {
            Value = value;
        }
    }
}