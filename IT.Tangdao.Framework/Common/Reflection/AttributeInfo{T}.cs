using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Reflection
{
    public readonly struct AttributeInfo<T> where T : Attribute
    {
        public readonly Type Type;
        public readonly T Attribute;

        public AttributeInfo(Type type, T attribute)
        {
            Type = type;
            Attribute = attribute;
        }
    }
}