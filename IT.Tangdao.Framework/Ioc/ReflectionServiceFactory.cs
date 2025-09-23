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
            // ① 选“参数最齐”的 public 构造函数
            var ctor = entry.ImplementationType
                            .GetConstructors()
                            .OrderByDescending(c => c.GetParameters().Length)
                            .FirstOrDefault()
                      ?? throw new InvalidOperationException($"类型 {entry.ImplementationType} 无可用的公共构造函数。");

            // ② 递归解析参数
            var args = ctor.GetParameters()
                           .Select(p => _provider.GetService(p.ParameterType)
                                      ?? throw new InvalidOperationException(
                                             $"构造函数参数 {p.ParameterType} 未注册。"))
                           .ToArray();

            // ③ 创建实例
            return ctor.Invoke(args);
        }
    }
}