using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 内部专用：把“key”附加在原有条目上，不污染公开接口。
    /// </summary>
    internal sealed class KeyedServiceEntry : IServiceEntry
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public ILifecycleStrategy LifecycleStrategy { get; }

        public object Key { get; }

        public KeyedServiceEntry(Type serviceType,
                                 Type implementationType,
                                 ILifecycleStrategy lifecycleStrategy,
                                 object key)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            LifecycleStrategy = lifecycleStrategy ?? throw new ArgumentNullException(nameof(lifecycleStrategy));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}