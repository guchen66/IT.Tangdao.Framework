using IT.Tangdao.Framework.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    internal struct TangdaoValue<T> : ITangdaoValue
    {
        private readonly T _value;

        public TangdaoValue(T value) => _value = value;

        public T Value => _value;

        object ITangdaoValue.GetUntyped() => _value;
    }
}