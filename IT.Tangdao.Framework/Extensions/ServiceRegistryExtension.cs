using IT.Tangdao.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 提供 <see cref="IServiceRegistry"/> 的依赖验证扩展方法。
    /// 在应用启动阶段遍历所有已注册的服务条目，检查其实现类的构造函数依赖是否完整注册。
    /// </summary>
    internal static class ServiceRegistryExtensions
    {
        /// <summary>
        /// 验证注册表中的所有服务依赖是否完整。
        /// 对每个已注册的服务条目，检查其实现类的最长构造函数的所有参数类型是否已在容器中注册。
        /// 如果发现依赖未注册，将抛出 <see cref="InvalidOperationException"/>。
        /// </summary>
        /// <param name="registry">待验证的服务注册表实例。</param>
        /// <exception cref="InvalidOperationException">当某个服务实现依赖了未在容器中注册的类型时抛出。</exception>
        public static void ValidateDependencies(this IServiceRegistry registry)
        {
            var visitor = new DependencyValidationVisitor(registry);
            foreach (var entry in registry.GetAllEntries())
            {
                visitor.Visit(entry);
            }
        }
    }
}