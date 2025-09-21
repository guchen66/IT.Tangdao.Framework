using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoCommon
{
    /// <summary>
    /// 同步数据传输，进行两个类之间简单的数据交互
    /// </summary>
    public static class TangdaoContext
    {
        /// <summary>
        /// 存储字符串
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _localValues = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 存储方法
        /// </summary>
        private static readonly ConcurrentDictionary<string, Delegate> _actions = new ConcurrentDictionary<string, Delegate>();

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

        public static void SetLocalValue(string name, string value)
        {
            _localValues[name] = value;
        }

        public static string GetLocalValue(string name)
        {
            _localValues.TryGetValue(name, out var value);
            return value;
        }

        // 存储Action的方法
        public static void SetLocalAction(string name, Action action)
        {
            _actions[name] = action;
        }

        // 执行存储的Action的方法
        public static void GetLocalAction(string name)
        {
            if (_actions.TryGetValue(name, out var action))
            {
                // 确保存储的是Action类型
                if (action is Action)
                {
                    (action as Action)?.Invoke();
                }
            }
        }

        public static void SetContext<TService>(RegisterContext context)
        {
            _contexts[typeof(TService)] = context;
        }

        public static RegisterContext GetContext<TService>()
        {
            _contexts.TryGetValue(typeof(TService), out var context);
            return context;
        }

        public static RegisterContext GetContext(Type type)
        {
            _contexts.TryGetValue(type, out var context);
            return context;
        }

        public static void SetInstance<TService>(TService instance)
        {
            _instances[typeof(TService)] = new Lazy<object>(() => instance);   // 立即返回已知的实例
        }

        public static TService GetInstance<TService>()
        {
            return _instances.TryGetValue(typeof(TService), out var lazy)
                ? (TService)lazy.Value   // 要么拿到实例，要么 null
                : default;
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