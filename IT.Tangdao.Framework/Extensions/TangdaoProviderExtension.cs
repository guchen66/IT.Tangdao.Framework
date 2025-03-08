using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.DaoEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoProviderExtension
    {
        public static TService Resolve<TService>(this ITangdaoProvider provider)
        {
            var context = ChannelEvent.GetContext<TService>();
            if (context == null)
            {
                throw new InvalidOperationException($"Unable to resolve type: {typeof(TService)}");
            }

            // 检查是否正在解析，避免循环依赖
            if (context.IsResolving)
            {
                throw new InvalidOperationException($"Circular dependency detected while resolving type: {typeof(TService)}");
            }

            context.IsResolving = true;

            try
            {
                // 如果请求的类型是接口，则查找对应的实现类
                Type implementationType = typeof(TService);
                if (typeof(TService).IsInterface)
                {
                    if (!context.InterfaceToImplementationMapping.TryGetValue(typeof(TService), out implementationType))
                    {
                        throw new InvalidOperationException($"No implementation registered for interface: {typeof(TService)}");
                    }
                }

                // 解析构造函数参数
                object[] parameterValues = new object[context.ParameterInfos.Length];
                for (int i = 0; i < context.ParameterInfos.Length; i++)
                {
                    var parameter = context.ParameterInfos[i];
                    Type parameterType = parameter.ParameterType;

                    // 递归解析参数类型
                    parameterValues[i] = ResolveParameter(provider, parameterType);
                }
                /*  context.FactoryMapping.TryAdd(typeof(TService), Activator.CreateInstance(implementationType, parameterValues));

                  if (context.Lifecycle == Lifecycle.Transient)
                  {
                      context.FactoryMapping.TryGetValue(typeof(TService), out object backType);
                      return (TService)backType;
                  }*/
                // 创建实例
                return (TService)Activator.CreateInstance(implementationType, parameterValues);
            }
            finally
            {
                context.IsResolving = false; // 重置标记
            }
        }

        private static object ResolveParameter(ITangdaoProvider provider, Type parameterType)
        {
            // 如果是基本类型或字符串，返回默认值
            if (parameterType == typeof(int))
            {
                return 0;
            }
            else if (parameterType == typeof(string))
            {
                return "default";
            }
            else if (parameterType == typeof(bool))
            {
                return false;
            }
            else if (parameterType.IsClass || parameterType.IsInterface)
            {
                // 如果参数类型是接口，则从映射中查找对应的实现类
                if (parameterType.IsInterface)
                {
                    var context = ChannelEvent.GetContext(parameterType);
                    object[] parameterValues = new object[context.ParameterInfos.Length];
                    if (context == null || !context.InterfaceToImplementationMapping.TryGetValue(parameterType, out var implementationType))
                    {
                        throw new InvalidOperationException($"No implementation registered for interface: {parameterType}");
                    }

                    // 递归解析实现类
                    // return provider.Resolve(implementationType);
                    // 解析构造函数参数

                    for (int i = 0; i < context.ParameterInfos.Length; i++)
                    {
                        var parameter = context.ParameterInfos[i];
                        parameterType = parameter.ParameterType;

                        // 递归解析参数类型
                        parameterValues[i] = ResolveParameter(provider, parameterType);
                    }

                    // 创建实例
                    return Activator.CreateInstance(implementationType, parameterValues);
                }

                // 如果是类，则直接递归解析
                return provider.Resolve(parameterType);
            }
            else if (parameterType.IsValueType)
            {
                // 创建值类型的默认实例
                return Activator.CreateInstance(parameterType);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported parameter type: {parameterType}");
            }
        }
    }
}