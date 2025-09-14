using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.DaoException;
using IT.Tangdao.Framework.DaoMvvm;
using IT.Tangdao.Framework.Extensions;

namespace IT.Tangdao.Framework
{
    public sealed class TangdaoContainer : ITangdaoContainer
    {
        /// <summary>
        /// 注册一个类
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public ITangdaoContainerBuilder Register<TService>()
        {
            Type serviceType = typeof(TService);

            var constructors = serviceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var constructor = constructors[0];
            var parameters = constructor.GetParameters();

            RegisterContext context = new RegisterContext
            {
                RegisterType = serviceType,
                ParameterInfos = parameters,
                Lifecycle = Lifecycle.Transient,
            };

            TangdaoContext.SetContext<TService>(context);
            return this;
        }

        public ITangdaoContainerBuilder Register<TService, TImplementation>() where TImplementation : TService
        {
            Type serviceType = typeof(TService);
            Type implementationType = typeof(TImplementation);

            var constructors = implementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var constructor = constructors[0]; // 使用第一个构造函数
            var parameters = constructor.GetParameters();

            RegisterContext context = new RegisterContext
            {
                RegisterType = implementationType,
                ParameterInfos = parameters
            };

            // 存储接口和实现类的映射
            context.InterfaceToImplementationMapping[serviceType] = implementationType;

            TangdaoContext.SetContext<TService>(context);
            return this;
        }

        public ITangdaoProvider Builder()
        {
            // 创建一个根提供者，不需要传递任何上下文
            return TangdaoContainerBuilder.Builder();
        }

        public ITangdaoContainer Register(Type serviceType, Type implementationType)
        {
            return this;
        }

        public ITangdaoContainer Register(Type implementationType)
        {
            RegisterContext context = new RegisterContext
            {
                RegisterType = implementationType,
            };
            return this;
        }

        public ITangdaoContainer Register(Type serviceType, Func<object> creator)
        {
            return this;
        }

        public ITangdaoContainer Register(Type type, Func<ITangdaoProvider, object> factoryMethod)
        {
            return this;
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

        public void Register<TService>(Func<ITangdaoContainer, TService> factory)
        {
            //_factories[typeof(TService)] = () => factory(this);
        }
    }
}