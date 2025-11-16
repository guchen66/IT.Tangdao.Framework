using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xaml;

namespace IT.Tangdao.Framework.Markup
{
    /// <summary>
    /// 环境传播
    /// </summary>
    [ContentProperty(nameof(Value))]
    public class AmbientValueExtension : MarkupExtension
    {
        public object Value { get; set; }

        public override object ProvideValue(IServiceProvider sp)
        {
            // 0. 必备上下文
            var target = sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var schema = sp.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
            var ambient = sp.GetService(typeof(IAmbientProvider)) as IAmbientProvider;
            if (schema == null || ambient == null) return Value;

            // 1. 把自己转成 XamlType
            var selfXamlType = schema.SchemaContext.GetXamlType(typeof(AmbientValueExtension));

            // 2. 继承逻辑
            if (target?.TargetObject == target?.TargetProperty) // 外层赋值节点
                return Value;

            // 3. 内层节点：从 ambient 拿
            var ambientInstance = ambient.GetFirstAmbientValue(selfXamlType);
            return ambientInstance;
        }
    }
}