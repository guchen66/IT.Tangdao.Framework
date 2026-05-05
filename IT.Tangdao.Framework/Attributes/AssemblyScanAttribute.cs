using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    /// <summary>
    /// 可扫描程序集，，标注特性后可主动扫描指定程序集，进行操作
    /// 操作1：对此程序集下的View，ViewModel进行绑定
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AssemblyScanAttribute : Attribute
    {
    }
}