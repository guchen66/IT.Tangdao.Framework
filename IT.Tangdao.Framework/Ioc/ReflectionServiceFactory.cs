using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 反射工厂：无缓存、无状态，线程安全。
    /// 构造函数参数通过传入的 ITangdaoProvider 递归解析。
    /// </summary>
    public sealed class ReflectionServiceFactory : IServiceFactory
    {
        private ITangdaoProvider _provider;

        internal void RebindProvider(ITangdaoProvider provider) => _provider = provider;

        public ReflectionServiceFactory(ITangdaoProvider provider = null)
        {
            _provider = provider;
        }

        public object Create(IServiceEntry entry)
        {
            // ① 尝试找到无参数的公共构造函数
            var ctor = entry.ImplementationType.GetConstructor(Type.EmptyTypes);

            // ② 如果没有无参数构造函数，再找参数最齐的公共构造函数
            if (ctor == null)
            {
                ctor = entry.ImplementationType
                        .GetConstructors()
                        .OrderByDescending(c => c.GetParameters().Length)
                        .FirstOrDefault()
                  ?? throw new InvalidOperationException($"类型 {entry.ImplementationType} 无可用的公共构造函数。");
            }

            // ③ 递归解析参数
            var args = new List<object>();
            foreach (var param in ctor.GetParameters())
            {
                // 对于基本类型和字符串，不尝试从容器解析，使用默认值
                if (param.ParameterType.IsPrimitive || param.ParameterType == typeof(string) || param.ParameterType == typeof(Type))
                {
                    // 使用参数的默认值
                    var defaultValue = param.HasDefaultValue ? param.DefaultValue : null;
                    args.Add(defaultValue);
                }
                else
                {
                    // 对于引用类型，从容器中解析
                    var service = _provider.GetService(param.ParameterType);
                    if (service == null)
                    {
                        throw new InvalidOperationException(
                            $"服务 '{entry.ServiceType}' 的实现 '{entry.ImplementationType}' 依赖未注册接口 '{param.ParameterType}'.");
                    }
                    args.Add(service);
                }
            }

            // ④ 创建实例
            return ctor.Invoke(args.ToArray());
        }
    }
}