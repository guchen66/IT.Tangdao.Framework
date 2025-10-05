using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace IT.Tangdao.Framework.Markup
{
    public class TypeOfExtension : MarkupExtension
    {
        public string TypeName { get; set; }

        //public TypeOfExtension(string typeName) => TypeName = typeName;

        public override object ProvideValue(IServiceProvider sp)
        {
            var resolver = sp.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
            return resolver?.Resolve(TypeName) ?? typeof(object);
        }
    }
}