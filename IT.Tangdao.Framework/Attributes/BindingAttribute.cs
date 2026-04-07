using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IT.Tangdao.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class BindingAttribute : Attribute
    {
        public string Path { get; set; }
        public BindingMode Mode { get; set; } = BindingMode.Default;
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; } = UpdateSourceTrigger.Default;
        public Type Converter { get; set; }
        public bool ContentControl { get; set; }
        public bool Owner { get; set; }
        public Type Dependency { get; set; }
        public Type Source { get; set; }
        public bool Lazy { get; set; }
        public string Group { get; set; }
    }
}