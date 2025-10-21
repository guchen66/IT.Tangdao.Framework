using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Events
{
    // 1. 引入强类型委托（替代Action<object>）
    public delegate void DaoEventHandler<in T>(T @event) where T : DaoEventBase;
}