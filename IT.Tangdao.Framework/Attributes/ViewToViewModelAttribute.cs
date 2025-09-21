using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    /// <summary>
    /// View与ViewModel关联特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewToViewModelAttribute : Attribute
    {
        public string ViewName { get; }

        public ViewToViewModelAttribute(string viewName)
        {
            ViewName = viewName;
        }
    }
}