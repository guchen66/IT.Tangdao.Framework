using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAttributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScanningAttribute : Attribute
    {
        public string RegisterType { get; set; }

    }
}
