using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using IT.Tangdao.Framework.Ioc;
using System.ComponentModel;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// 默认只写容器，线程安全委托给 ServiceRegistry。
    /// 本身不做任何校验，保持单一职责。
    /// </summary>
    public sealed class TangdaoContainer : ITangdaoContainer
    {
        public IServiceRegistry Registry { get; }

        public TangdaoContainer()
        {
            Registry = new ServiceRegistry();
        }

        public void Register(IServiceEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            Registry.Add(entry);
        }

        public ITangdaoProvider BuildProvider()
        {
            // 把内部注册表和反射工厂一起塞进 Provider
            var factory = new ReflectionServiceFactory(
                            new TangdaoProvider(Registry, null)); // 先循环引用占位
                                                                  // 用临时委托解决循环：Provider 需要 Factory，Factory 需要 Provider
            var provider = new TangdaoProvider(Registry, factory);
            ((ReflectionServiceFactory)factory).RebindProvider(provider);
            return provider;
        }

        // 内部延迟队列
        private readonly List<Action<ITangdaoContainer>> _lazy = new List<Action<ITangdaoContainer>>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<Action<ITangdaoContainer>> LazyRegistrations => _lazy;
    }
}