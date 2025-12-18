using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Events.Delegates
{
    public delegate void TangdaoEventTranstionPipe<TKey, TEventArgs>(object sender, TEventArgs e);
}