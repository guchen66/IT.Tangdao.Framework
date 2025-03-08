using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoInterfaces
{
    public interface INavigationAware
    {
        void OpenWindowExecute(ITangdaoParameter tangdaoParameter);
    }
}