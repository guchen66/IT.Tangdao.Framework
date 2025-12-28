using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IT.Tangdao.Framework.Helpers
{
    public static class XmlMapper<T> where T : new()
    {
        // 缓存：XName→编译后的赋值委托
        private static readonly Dictionary<string, Action<XElement, T>> _map;

        static XmlMapper()
        {
            var paramElem = Expression.Parameter(typeof(XElement), "e");
            var paramInst = Expression.Parameter(typeof(T), "inst");

            var dict = new Dictionary<string, Action<XElement, T>>();
            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                          .Where(p => p.CanWrite))
            {
                var name = prop.Name;
                var elem = Expression.Call(paramElem,
                                            typeof(XElement).GetMethod("Element", new[] { typeof(XName) }),
                                            Expression.Constant(name, typeof(XName)));
                // e.Element(name)
                var nullCheck = Expression.Equal(elem, Expression.Constant(null, typeof(XElement)));
                // value = (TProp)Convert.ChangeType(elem.Value, typeof(TProp))
                var valueExpr = Expression.Call(
                                    typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) }),
                                    Expression.Property(elem, "Value"),
                                    Expression.Constant(prop.PropertyType, typeof(Type)));
                var castExpr = Expression.Convert(valueExpr, prop.PropertyType);
                // inst.Prop = value
                var assign = Expression.Assign(Expression.Property(paramInst, prop), castExpr);
                var body = Expression.IfThen(Expression.Not(nullCheck), assign);
                var lambda = Expression.Lambda<Action<XElement, T>>(body, paramElem, paramInst);
                dict[name] = lambda.Compile();
            }
            _map = dict;
        }

        public static T Map(XElement node)
        {
            var inst = new T();
            foreach (var kv in _map) kv.Value(node, inst);
            return inst;
        }
    }
}