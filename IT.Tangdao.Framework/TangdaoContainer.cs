using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IT.Tangdao.Framework.Extensions;
namespace IT.Tangdao.Framework
{
    public class TangdaoContainer : ITangdaoContainer
    {
        private readonly Dictionary<Type,Func<object>> _containerClasss= new Dictionary<Type,Func<object>>();

        private readonly Dictionary<Type, object> _containerInterface = new Dictionary<Type, object>();

        public ITangdaoContainer GetContainer()
        {
            throw new NotImplementedException();
        }

        public void RegisterScoped<TType>(params object[] obj)
        {
            _containerClasss[typeof(TType)]=()=>Activator.CreateInstance(typeof(TType),obj);           //仅仅适用于无参构造器
           // Assembly.GetExecutingAssembly();       //当前正在执行的代码的程序集
        }

        public void RegisterScoped<TType, TypeImple>() where TypeImple : TType
        {
            _containerInterface[typeof(TType)]=typeof(TypeImple);
        }

        public TType Resolve<TType>()
        {
            Type type = typeof(TType);
            if (_containerClasss.TryGetValue(type, out Func<object> func))
            {
                return (TType)func();
            }
            throw new InvalidOperationException($"类型{typeof(TType)} 没有注册.");
        }
    }
}
