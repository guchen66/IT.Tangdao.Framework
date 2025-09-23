using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// “只读”解析器：从注册表获取策略与工厂，完成实例化。
    /// 不暴露任何写入能力。
    /// </summary>
    public interface ITangdaoProvider
    {
        /// <summary>
        /// 获取服务；找不到返回 null。
        /// </summary>
        object GetService(Type serviceType);

        /// <summary>
        /// 泛型便利方法，由扩展方法默认实现。
        /// </summary>
        T GetService<T>() where T : class;
    }
}