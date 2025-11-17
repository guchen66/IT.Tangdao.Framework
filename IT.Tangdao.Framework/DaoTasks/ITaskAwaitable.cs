using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    internal interface ITaskAwaitable
    {
        void OnCompleted();

        void OnFaulted(Exception ex);
    }
}