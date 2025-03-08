using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public interface IDaoCloneable<out T>
    {
        T Clone(); 
    }
}
