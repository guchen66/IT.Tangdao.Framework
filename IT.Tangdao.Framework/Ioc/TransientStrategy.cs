using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 瞬态策略：每次调用都通过工厂重新创建实例。
    /// 无状态，线程安全。
    /// </summary>
    public sealed class TransientStrategy : ILifecycleStrategy
    {
        public object CreateInstance(ServiceCreationContext context)
        {
            // 直接委托给工厂，不缓存
            return context.Factory.Create(context.Entry);
        }
    }
}