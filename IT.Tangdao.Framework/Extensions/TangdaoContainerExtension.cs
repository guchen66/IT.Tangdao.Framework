using System;
using IT.Tangdao.Framework.Ioc;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 给 ITangdaoContainer 加链式语法糖，全部转发到 Register 方法。
    /// </summary>
    public static class TangdaoContainerExtension
    {
        public static ITangdaoContainer RegisterTransient<TService, TImpl>(this ITangdaoContainer container) where TImpl : TService
        {
            container.Register(new ServiceEntry(typeof(TService), typeof(TImpl), new TransientStrategy()));
            return container;
        }

        public static ITangdaoContainer RegisterSingleton<TService, TImpl>(this ITangdaoContainer container) where TImpl : TService
        {
            container.Register(new ServiceEntry(typeof(TService), typeof(TImpl), new SingletonStrategy()));
            return container;
        }

        /// <summary>
        /// 自己注册自己：T 既是服务类型也是实现类型。
        /// </summary>
        public static ITangdaoContainer RegisterTransient<T>(this ITangdaoContainer container) where T : class
        {
            return container.RegisterTransient(typeof(T));
        }

        public static ITangdaoContainer RegisterTransient(this ITangdaoContainer container, Type type)
        {
            container.Register(new ServiceEntry(type, type, new TransientStrategy()));
            return container;
        }

        public static ITangdaoContainer RegisterSingleton<T>(this ITangdaoContainer container) where T : class
        {
            return container.RegisterSingleton(typeof(T));
        }

        public static ITangdaoContainer RegisterSingleton(this ITangdaoContainer container, Type type)
        {
            container.Register(new ServiceEntry(type, type, new SingletonStrategy()));
            return container;
        }

        #region 作用域注册

        /// <summary>
        /// 自己注册自己（作用域）
        /// </summary>
        public static ITangdaoContainer RegisterScoped<T>(this ITangdaoContainer container) where T : class
        {
            return container.RegisterScoped(typeof(T));
        }

        public static ITangdaoContainer RegisterScoped(this ITangdaoContainer container, Type type)
        {
            container.Register(new ServiceEntry(type, type, new ScopedStrategy()));
            return container;
        }

        /// <summary>
        /// 接口→实现（作用域）
        /// </summary>
        public static ITangdaoContainer RegisterScoped<TService, TImpl>(this ITangdaoContainer container) where TImpl : TService
        {
            return container.RegisterScoped(typeof(TService), typeof(TImpl));
        }

        public static ITangdaoContainer RegisterScoped(this ITangdaoContainer container, Type serviceType, Type implementationType)
        {
            if (!serviceType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException(
                    $"类型 {implementationType.Name} 必须继承或实现 {serviceType.Name}");
            }
            container.Register(new ServiceEntry(serviceType, implementationType, new ScopedStrategy()));
            return container;
        }

        #endregion 作用域注册

        #region 工厂注册

        /// <summary>
        /// 瞬态工厂：每次调用 factory 委托。
        /// </summary>
        public static ITangdaoContainer RegisterTransientFactory<T>(this ITangdaoContainer container, Func<ITangdaoProvider, T> factory) where T : class
        {
            container.Register(new ServiceEntry(typeof(T), typeof(TransientFactory<T>), new TransientStrategy()));
            // 把委托塞进容器元数据，后续工厂能拿到
            TransientFactory<T>.SetFactory(factory);
            return container;
        }

        /// <summary>
        /// 单例工厂：只调用一次 factory 委托，结果缓存。
        /// </summary>
        public static ITangdaoContainer RegisterSingletonFactory<T>(this ITangdaoContainer container, Func<ITangdaoProvider, T> factory) where T : class
        {
            container.Register(new ServiceEntry(typeof(T), typeof(SingletonFactory<T>), new SingletonStrategy()));
            SingletonFactory<T>.SetFactory(factory);
            return container;
        }

        /// <summary>
        /// 把**已有实例**注册成单例；后续解析都返回同一对象。
        /// </summary>
        public static ITangdaoContainer RegisterSingleton<T>(this ITangdaoContainer container, T instance) where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            // 用匿名工厂包装，永远返回同一实例
            container.Register(new ServiceEntry(
                typeof(T),
                typeof(T),                  // 任意写，不会被用到
                new SingletonInstanceStrategy(instance))); // 见下
            return container;
        }

        /// <summary>
        /// 专门为“实例单例”写的生命周期策略——永远返回同一对象。
        /// </summary>
        private sealed class SingletonInstanceStrategy : ILifecycleStrategy
        {
            private readonly object _instance;

            public SingletonInstanceStrategy(object instance) => _instance = instance;

            public object CreateInstance(ServiceCreationContext context) => _instance;
        }

        #endregion 工厂注册

        #region 私有工厂实现

        /// <summary>
        /// 瞬态工厂包装
        /// </summary>
        private sealed class TransientFactory<T> : IServiceFactory where T : class
        {
            private static Func<ITangdaoProvider, T> _factory;

            internal static void SetFactory(Func<ITangdaoProvider, T> f) => _factory = f;

            public object Create(IServiceEntry entry)
            {
                if (_factory == null) throw new InvalidOperationException("瞬态工厂未配置。");
                return _factory(ProviderHolder.Provider);
            }
        }

        /// <summary>
        /// 单例工厂包装
        /// </summary>
        private sealed class SingletonFactory<T> : IServiceFactory where T : class
        {
            private static Func<ITangdaoProvider, T> _factory;
            private static T _cache;
            private static readonly object _lock = new object();

            internal static void SetFactory(Func<ITangdaoProvider, T> f) => _factory = f;

            public object Create(IServiceEntry entry)
            {
                if (_factory == null) throw new InvalidOperationException("单例工厂未配置。");

                lock (_lock)
                {
                    if (_cache == null)
                    {
                        _cache = _factory(ProviderHolder.Provider);
                    }
                    return _cache;
                }
            }
        }

        /// <summary>
        /// 工厂内部取 Provider 的桥梁
        /// </summary>
        private static class ProviderHolder
        {
            internal static ITangdaoProvider Provider => ServiceLocator.Default;
        }

        #endregion 私有工厂实现

        /// <summary>
        /// 框架私有的懒加载实现
        /// </summary>
        /// <param name="container"></param>
        /// <param name="registerAction"></param>
        /// <returns></returns>
        internal static ITangdaoContainer RegisterLazyRegistration(this ITangdaoContainer container, Action<ITangdaoContainer> registerAction)
        {
            container.LazyRegistrations.Add(registerAction);
            return container;   // 链式
        }

        #region 私有验证方法

        /// <summary>
        /// 验证类型是否为可实例化的具体类型（非接口、非抽象类）。
        /// </summary>
        /// <param name="type">待验证的类型</param>
        /// <exception cref="InvalidOperationException">当类型是接口或抽象类时抛出</exception>
        private static void ValidateConcreteType(Type type)
        {
            if (type.IsInterface)
            {
                throw new InvalidOperationException(
                    $"不能直接注册接口 '{type.Name}'。请使用 RegisterTransient<{type.Name}, TImpl>() 或类似重载指定具体实现类。");
            }

            if (type.IsAbstract)
            {
                throw new InvalidOperationException(
                    $"不能直接注册抽象类 '{type.Name}'。请使用 RegisterTransient<{type.Name}, TImpl>() 或类似重载指定具体实现类。");
            }
        }

        #endregion 私有验证方法
    }
}