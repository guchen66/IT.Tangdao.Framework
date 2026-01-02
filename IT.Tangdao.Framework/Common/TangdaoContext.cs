using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Contracts;

namespace IT.Tangdao.Framework.Common
{
    /// <summary>
    /// 同步数据传输，进行两个类之间简单的数据交互
    /// </summary>
    public static class TangdaoContext
    {
        /// <summary>
        /// 存储参数
        /// </summary>
        private static readonly ConcurrentDictionary<string, ITangdaoParameter> _parameter = new ConcurrentDictionary<string, ITangdaoParameter>();

        /// <summary>
        /// 存储数据上下文
        /// </summary>
        private static readonly ConcurrentDictionary<Type, RegisterContext> _contexts = new ConcurrentDictionary<Type, RegisterContext>();

        /// <summary>
        /// 存储工厂
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Func<ITangdaoProvider, object>> _instanceFactories = new ConcurrentDictionary<Type, Func<ITangdaoProvider, object>>();

        /// <summary>
        /// 存储实例
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Lazy<object>> _instances = new ConcurrentDictionary<Type, Lazy<object>>();

        /// <summary>
        /// 存储“带键”实例（与类型无关，纯字符串 key）
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> _keyedInstances = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 设置全局参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="parameter">参数实例</param>
        public static void SetTangdaoParameter(string name, ITangdaoParameter parameter)
        {
            _parameter[name] = parameter;
        }

        /// <summary>
        /// 获取全局参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>参数实例，如果不存在则返回null</returns>
        public static ITangdaoParameter GetTangdaoParameter(string name)
        {
            _parameter.TryGetValue(name, out var value);
            return value;
        }

        /// <summary>
        /// 创建并存储包含RegistrationTypeEntry的参数实例
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="registrationTypeEntry">类型注册信息</param>
        /// <returns>创建的参数实例</returns>
        public static ITangdaoParameter CreateAndSetParameterWithTypeEntry(string name, IRegistrationTypeEntry registrationTypeEntry)
        {
            var parameter = new TangdaoParameter();
            parameter.Add("RegistrationTypeEntry", registrationTypeEntry);
            _parameter[name] = parameter;
            return parameter;
        }

        /// <summary>
        /// 从参数实例中获取RegistrationTypeEntry
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>类型注册信息，如果不存在则返回null</returns>
        public static IRegistrationTypeEntry GetRegistrationTypeEntryFromParameter(string name)
        {
            if (_parameter.TryGetValue(name, out var parameter))
            {
                return parameter.Get<IRegistrationTypeEntry>("RegistrationTypeEntry");
            }
            return null;
        }

        /// <summary>
        /// 设置类型的注册上下文
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="context">注册上下文</param>
        public static void SetContext<TService>(RegisterContext context)
        {
            _contexts[typeof(TService)] = context;
        }

        /// <summary>
        /// 获取类型的注册上下文
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns>注册上下文，如果不存在则返回null</returns>
        public static RegisterContext GetContext<TService>()
        {
            _contexts.TryGetValue(typeof(TService), out var context);
            return context;
        }

        /// <summary>
        /// 获取类型的注册上下文
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <returns>注册上下文，如果不存在则返回null</returns>
        public static RegisterContext GetContext(Type type)
        {
            _contexts.TryGetValue(type, out var context);
            return context;
        }

        /// <summary>
        /// 设置类型的单例实例
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">实例对象</param>
        public static void SetInstance<TService>(TService instance)
        {
            _instances[typeof(TService)] = new Lazy<object>(() => instance);   // 立即返回已知的实例
        }

        /// <summary>
        /// 获取类型的单例实例
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns>实例对象，如果不存在则返回default</returns>
        public static TService GetInstance<TService>()
        {
            return _instances.TryGetValue(typeof(TService), out var lazy)
                ? (TService)lazy.Value   // 要么拿到实例，要么 null
                : default;
        }

        /// <summary>
        /// 设置带键实例（与类型无关，纯 key-value）
        /// </summary>
        /// <param name="key">实例键</param>
        /// <param name="instance">实例对象</param>
        public static void SetInstance(string key, object instance)
        {
            _keyedInstances[key] = instance;
        }

        /// <summary>
        /// 按 key 获取实例；找不到返回 default(T)
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="key">实例键</param>
        /// <returns>实例对象，如果不存在则返回default</returns>
        public static T GetInstance<T>(string key) where T : class
        {
            _keyedInstances.TryGetValue(key, out var hit);
            return hit as T;
        }

        /// <summary>
        /// 注册一个类型的实例工厂方法。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="factory">工厂方法，接受 ITangdaoProvider 并返回 TService 实例。</param>
        public static void SetInstanceFactory<TService>(Func<ITangdaoProvider, object> factory)
        {
            _instanceFactories[typeof(TService)] = factory;
        }

        /// <summary>
        /// 获取指定类型的单例实例。如果尚未创建，则使用注册的工厂方法创建。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="provider">ITangdaoProvider 实例。</param>
        /// <returns>TService 的单例实例。</returns>
        public static TService GetInstanceFactory<TService>(ITangdaoProvider provider) where TService : class
        {
            var lazy = _instances.GetOrAdd(typeof(TService),
                t => new Lazy<object>(() =>
                {
                    if (_instanceFactories.TryGetValue(t, out var factory))
                        return factory(provider);
                    throw new InvalidOperationException(
                        $"No instance factory registered for type {t.FullName}");
                }, true));          // true = 使用线程安全模式
            return (TService)lazy.Value;
        }

        /// <summary>
        /// 获取指定类型的单例实例。如果尚未创建，则使用注册的工厂方法创建。
        /// </summary>
        /// <param name="type">服务类型。</param>
        /// <param name="provider">ITangdaoProvider 实例。</param>
        /// <returns>指定类型的单例实例。</returns>
        public static object GetInstanceFactory(Type type, ITangdaoProvider provider)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            var lazy = _instances.GetOrAdd(type,
                t => new Lazy<object>(() =>
                {
                    if (_instanceFactories.TryGetValue(t, out var factory))
                        return factory(provider);
                    throw new InvalidOperationException(
                        $"No instance factory registered for type {t.FullName}");
                }, true));
            return lazy.Value;
        }
    }
}