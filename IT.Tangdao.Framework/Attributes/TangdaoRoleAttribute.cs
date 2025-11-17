using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    /// <summary>
    /// Role特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TangdaoRoleAttribute : Attribute
    {
        public int Remark { get; private set; }

        public TangdaoRoleAttribute(int remark)
        {
            this.Remark = remark;
        }
    }
}