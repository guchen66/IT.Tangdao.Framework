using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    [Flags]
    public enum StringHandlerOption
    {
        None = 0,
        Trim = 1,
        ToUpper = 2,
        ToLower = 4,
        NullToEmpty = 8,
        EmptyToNull = 16
    }
}