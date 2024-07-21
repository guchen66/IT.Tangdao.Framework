using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public interface ITangdaoProvider
    {
        object Resolve(Type type);
    }
}
