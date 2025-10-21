using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 减少拆箱，装箱接口
    /// </summary>
    internal interface ITangdaoValue
    {
        object GetUntyped();
    }
}