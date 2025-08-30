using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Navigates
{
    public interface ISingleNavigateView
    {
        string ViewName { get; }
        int DisplayOrder { get; }
    }
}