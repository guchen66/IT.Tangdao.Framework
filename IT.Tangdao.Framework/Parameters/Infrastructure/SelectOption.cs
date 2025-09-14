using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Parameters.Infrastructure
{
    public abstract class SelectOption : IEquatable<SelectOption>
    {
        public string Name { get; }
        public int Value { get; }

        protected SelectOption(string name, int value)
        {
            Name = name;
            Value = value;
        }

        // 值相等性比较
        public bool Equals(SelectOption other) => other?.Value == Value;

        public override bool Equals(object obj) => Equals(obj as SelectOption);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Name;
    }
}