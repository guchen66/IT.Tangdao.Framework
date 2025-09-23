using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 一条注册信息：服务类型、实现类型、生命周期策略。
    /// 完全只读，供全局传递。
    /// </summary>
    public interface IServiceEntry
    {
        /// <summary>
        /// 要请求的服务类型（通常是接口）
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// 实现该服务的具体类型（必须可实例化）
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// 生命周期策略：Transient、Singleton...
        /// </summary>
        ILifecycleStrategy LifecycleStrategy { get; }
    }
}