using IT.Tangdao.Framework.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Windows
{
    public class LoginSignGuard : EmptySignGuard<GuardContext>
    {
        public bool Validate(GuardContext guardContext)
        {
            return true;
        }
    }
}