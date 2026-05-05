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
                // 对于值类型（包括枚举）、字符串和 Type，不尝试从容器解析，使用默认值
                if (IsValueTypeWithDefault(param.ParameterType))
                {
                    // 使用参数的默认值，值类型确保使用 default(T) 而非 null
                    var defaultValue = param.HasDefaultValue ? param.DefaultValue : GetDefaultValue(param.ParameterType);
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

        /// <summary>
        /// 判断类型是否为需要使用默认值的类型
        /// 包括：值类型（含枚举）、字符串、Type
        /// </summary>
        /// <param name="type">要判断的类型</param>
        /// <returns>是否为需要使用默认值的类型</returns>
        private bool IsValueTypeWithDefault(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsValueType || type == typeof(string) || type == typeof(Type);
        }

        /// <summary>
        /// 获取类型的默认值
        /// 对于值类型返回 default(T)，对于引用类型返回 null
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型的默认值</returns>
        private object GetDefaultValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}