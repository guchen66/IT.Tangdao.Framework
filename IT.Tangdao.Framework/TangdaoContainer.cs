using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.DaoComponents;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Providers;
namespace IT.Tangdao.Framework
{
    public class TangdaoContainer : TangdaoAdapter, ITangdaoContainer
    {
        public Dictionary<Type, object> SelfRegisterType { get; set; } = new Dictionary<Type, object>();
        public ITangdaoContainer Register(Type serviceType, Type implementationType)
        {
            var ctors = implementationType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).ToList();
            var ctor = ctors.First(); // 获取最匹配的构造函数
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType).ToArray();         //拿到所有的参数类型
            SelfRegisterType.Add(serviceType, parameterTypes);
            CurrentContext.Add(new CommonContext
            {
                RegisterType = SelfRegisterType,          //把构造器和它内部的参数传进来            //显示有字典试一下，字典不行换委托
                ServiceType = serviceType,               //把所有注册的接口传进来            因为解析的时候是从接口解析的，所以我需要获取接口
                ParameterTypes = parameterTypes,         //把需要构造器内部的接口传进来
                                                         //   Creator = (args) => creator() // 注意这里的变化
            });
            return new TangdaoContainer();
            //  return Register(CurrentContext);           
        }


        public ITangdaoContainer Register(Type serviceType, Func<object> creator)
        {
            return new TangdaoContainer();
        }


        private ITangdaoContainer Register(List<CommonContext> commonContexts)
        {
            // var runtimeType = serviceType.UnderlyingSystemType;
            // Register(serviceType, Activator.CreateInstance(implementationType, obj));
            //   var parameterResolvers = parameterTypes.Select(ptype => (Func<object[], object>)((args) => Resolve(ptype))).ToArray();

            //  Register(serviceType, parameterTypes, () => ctor.Invoke(parameterResolvers.Select(fr => fr(null)).ToArray()));

            return new TangdaoContainer();
        }

        public ITangdaoContainer Register(Type type, Func<ITangdaoProvider, object> factoryMethod)
        {
            return new TangdaoContainer();
        }

        // 用于跟踪当前正在解析的类型，以避免递归
        private Stack<Type> ResolvingTypes { get; } = new Stack<Type>();
    }
}
