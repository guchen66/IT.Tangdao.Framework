using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Reflection
{
    /// <summary>
    /// 非泛型AttributeInfo
    /// </summary>
    public struct AttributeInfo
    {
        public Type Type { get; }
        public Attribute Attribute { get; }

        public AttributeInfo(Type type, Attribute attribute)
        {
            Type = type;
            Attribute = attribute;
        }
    }
}