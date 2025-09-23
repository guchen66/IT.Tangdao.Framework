using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 注册阶段一次性校验：确保“能解析到底”。
    /// 只读、无状态，可多次重用。
    /// </summary>
    public sealed class DependencyValidationVisitor : IRegistryVisitor
    {
        private readonly IServiceRegistry _registry;

        public DependencyValidationVisitor(IServiceRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public void Visit(IServiceEntry entry)
        {
            // ① 取“最长”构造函数
            var ctor = entry.ImplementationType
                            .GetConstructors()
                            .OrderByDescending(c => c.GetParameters().Length)
                            .FirstOrDefault();

            if (ctor == null) return;   // 无参构造直接过

            // ② 逐个参数检查
            foreach (var param in ctor.GetParameters())
            {
                if (_registry.GetEntry(param.ParameterType) == null)
                {
                    throw new InvalidOperationException(
                        $"服务 '{entry.ServiceType.Name}' 的实现 '{entry.ImplementationType.Name}' " +
                        $"依赖未注册接口 '{param.ParameterType.Name}'。");
                }
            }
        }
    }
}