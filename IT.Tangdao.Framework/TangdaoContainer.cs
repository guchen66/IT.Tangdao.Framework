using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.DaoComponents;
using IT.Tangdao.Framework.DaoDtos.Globals;
using IT.Tangdao.Framework.DaoException;
using IT.Tangdao.Framework.DaoMvvm;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Providers;
namespace IT.Tangdao.Framework
{
    public sealed class TangdaoContainer : ITangdaoAdapter, ITangdaoContainer
    {
        private readonly List<CommonContext> _contexts;

        public TangdaoContainer()
        {
            _contexts = new List<CommonContext> { new CommonContext() };
        }

        public List<CommonContext> GetContexts()
        {
            return _contexts;
        }

        public ITangdaoContainer Register(Type serviceType, Type implementationType)
        {
            var context = _contexts.LastOrDefault();
            if (context == null)
            {
                throw new InvalidOperationException("No context available for registration.");
            }

            if (!context.RegisterType.ContainsKey(serviceType))
            {
                context.RegisterType[serviceType] = implementationType; // 注册服务类型到实现类型
            }
            else
            {
                // 如果服务类型已经存在，则抛出异常或者进行其他处理
                throw new InvalidOperationException($"Service type {serviceType.FullName} is already registered.");
            }

            return this;
        }

        public ITangdaoContainer Register(Type implementationType)
        {
            var context = _contexts.LastOrDefault();
            if (context == null)
            {
                throw new InvalidOperationException("No context available for registration.");
            }

            if (!context.RegisterType.ContainsKey(implementationType))
            {
                context.RegisterType[implementationType] = implementationType; // 注册实现类型
            }
            else
            {
                // 如果实现类型已经存在，则抛出异常或者进行其他处理
                throw new InvalidOperationException($"Implementation type {implementationType.FullName} is already registered.");
            }

            return this;
        }

      
        public ITangdaoContainer Register(Type serviceType, Func<object> creator)
        {
            var context = _contexts.LastOrDefault();
            if (context == null)
            {
                throw new InvalidOperationException("No context available for registration.");
            }

            if (!context.RegisterType.ContainsKey(serviceType))
            {
                context.RegisterType.Add(serviceType, creator); // 注册服务类型到创建函数
            }
            else
            {
                // 如果服务类型已经存在，则抛出异常或者进行其他处理
                throw new InvalidOperationException($"Service type {serviceType.FullName} is already registered.");
            }

            return this;
        }

        public ITangdaoContainer Register(Type type, Func<ITangdaoProvider, object> factoryMethod)
        {
            var context = _contexts.LastOrDefault();
            if (context == null)
            {
                throw new InvalidOperationException("No context available for registration.");
            }

            if (!context.RegisterType.ContainsKey(type))
            {
                context.RegisterType.Add(type, factoryMethod); // 注册类型到工厂方法
            }
            else
            {
                // 如果类型已经存在，则抛出异常或者进行其他处理
                throw new InvalidOperationException($"Type {type.FullName} is already registered.");
            }

            return this;
        }

        // 如果需要添加新的上下文，可以添加一个方法来处理
        public void AddContext()
        {
            _contexts.Add(new CommonContext());
        }

        public ITangdaoContainer Register(string name)
        {
            if (name is string model)
            {
                IEnumerable<Type> types = ViewToViewModelExtension.GetScanObject(name);
                return ViewToViewModelLocator.Build(types);
            }
          
            throw new ContainerErrorException("注册ViewToModel未提供Name");
        }

        // 用于跟踪当前正在解析的类型，以避免递归
        private Stack<Type> ResolvingTypes { get; } = new Stack<Type>();
    }
}
