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

        public ServiceEntry(Type serviceType, Type implementationType, ILifecycleStrategy lifecycleStrategy)
        {
            // 防御式校验，早失败、易排错
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));
            if (lifecycleStrategy == null)
                throw new ArgumentNullException(nameof(lifecycleStrategy));
            if (!serviceType.IsAssignableFrom(implementationType))
                throw new ArgumentException($"类型 '{implementationType}' 未实现/继承 '{serviceType}'.");

            ServiceType = serviceType;
            ImplementationType = implementationType;
            LifecycleStrategy = lifecycleStrategy;
        }
    }
}