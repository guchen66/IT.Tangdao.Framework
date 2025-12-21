using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// IServiceEntry 的不可变实现。
    /// 构造时做一次基本校验，后续任意线程安全使用。
    /// </summary>
    public sealed class ServiceEntry : IServiceEntry
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public ILifecycleStrategy LifecycleStrategy { get; }
        public Func<ITangdaoProvider, object> Factory { get; }

        public ServiceEntry(Type serviceType, Type implementationType, ILifecycleStrategy lifecycleStrategy)
        {
            // 防御式校验，早失败、易排错
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));
            if (lifecycleStrategy == null)
                throw new ArgumentNullException(nameof(lifecycleStrategy));

            // 跳过类型检查的特殊情况：实现类型是工厂类型（实现了 IServiceFactory）
            // 这种情况下，实际创建实例的是工厂，而不是 ImplementationType 本身
            if (!typeof(IServiceFactory).IsAssignableFrom(implementationType) &&
                !serviceType.IsAssignableFrom(implementationType))
                throw new ArgumentException($"类型 '{implementationType}' 未实现/继承 '{serviceType}'.");

            ServiceType = serviceType;
            ImplementationType = implementationType;
            LifecycleStrategy = lifecycleStrategy;
        }
    }
}