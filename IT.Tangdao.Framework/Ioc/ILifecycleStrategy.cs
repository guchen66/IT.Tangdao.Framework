using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 生命周期策略的“唯一契约”：
    /// 根据传入的注册项和解析上下文，返回一个实例。
    /// 实现类可自由选择“每次都 new”还是“缓存复用”。
    /// </summary>
    public interface ILifecycleStrategy
    {
        object CreateInstance(ServiceCreationContext context);
    }
}