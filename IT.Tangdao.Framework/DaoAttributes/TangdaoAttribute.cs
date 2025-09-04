using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAttributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TangdaoAttribute : Attribute
    {
        public string Name { get; set; }

        public TangdaoAttribute()
        {
        }
    }
}