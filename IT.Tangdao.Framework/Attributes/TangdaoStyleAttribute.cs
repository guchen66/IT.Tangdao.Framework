using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace IT.Tangdao.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TangdaoStyleAttribute : Attribute
    {
        public string StyleName { get; set; }
        public ControlTemplate StyleTemplate { get; set; }
        public DataTemplate ContentTemplate { get; set; }
    }
}