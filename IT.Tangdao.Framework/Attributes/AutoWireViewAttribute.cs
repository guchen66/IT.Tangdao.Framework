using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    /// <summary>
    /// 对ViewModel设置特性，自动关联View
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutoWireViewAttribute : Attribute
    {
    }
}