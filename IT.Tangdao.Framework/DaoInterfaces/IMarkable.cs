using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoInterfaces
{
    internal interface IMarkable
    {
        void MarkCompleted();

        void MarkFaulted(Exception ex);
    }
}