using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace IT.Tangdao.Framework.Markup
{
    public class EnumBindSourceExtension : MarkupExtension
    {
        private Type _enumType;

        public Type EnumType
        {
            get => _enumType;
            set => _enumType = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider), "serviceProvider 不能为 null");
            }
            var actualType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
            var enumType = Enum.GetValues(_enumType);
            if (actualType == _enumType)
            {
                return enumType;
            }
            var tempArray = Array.CreateInstance(actualType, enumType.Length + 1);
            return tempArray;
        }
    }
}