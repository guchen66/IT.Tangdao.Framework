using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    /// <summary>
    /// 可扫描程序集,标注特性后可主动扫描指定程序集，进行操作(Component开发，功能待定)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AssemblyScanAttribute : Attribute
    {
    }
}