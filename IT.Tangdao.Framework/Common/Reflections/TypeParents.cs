using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common.Reflections
{
    public readonly struct TypeParents
    {
        public TypeParents(Type baseClass, IReadOnlyList<Type> interfaces)
        {
            BaseClass = baseClass;
            Interfaces = interfaces ?? Array.Empty<Type>();
        }

        public Type BaseClass { get; }           // 无 set，只读
        public IReadOnlyList<Type> Interfaces { get; }

        public void Deconstruct(out Type baseClass, out IReadOnlyList<Type> interfaces)
        {
            baseClass = BaseClass;
            interfaces = Interfaces;
        }
    }
}