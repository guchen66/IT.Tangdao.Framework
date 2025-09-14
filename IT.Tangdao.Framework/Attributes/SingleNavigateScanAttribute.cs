using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SingleNavigateScanAttribute : Attribute
    {
        public string ViewScanName { get; }

        public SingleNavigateScanAttribute(string viewScanName)
        {
            ViewScanName = viewScanName;
        }
    }
}