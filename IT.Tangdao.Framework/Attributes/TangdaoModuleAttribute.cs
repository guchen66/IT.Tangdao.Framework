using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class TangdaoModuleAttribute : Attribute
    {
        public Type ModuleType { get; }

        public TangdaoModuleAttribute(Type moduleType) =>
            ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
    }
}