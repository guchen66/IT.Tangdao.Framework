using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 服务定位器：提供全局访问服务的静态入口
    /// </summary>
    public sealed class ServiceLocator : ITangdaoProvider
    {
        /// <summary>
        /// 获取默认服务定位器实例
        /// </summary>
        public static ServiceLocator Default { get; } = new ServiceLocator();

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>服务实例，未找到返回null</returns>
        public object GetService(Type serviceType)
        {
            TangdaoGuards.ThrowIfNull(serviceType, nameof(serviceType));
            var provider = _provider;
            if (provider == null)
            {
                throw new InvalidOperationException("服务提供者尚未初始化，请先调用Initialize方法");
            }
            return provider.GetService(serviceType);
        }

        /// <summary>
        /// 获取指定类型的服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例，未找到返回null</returns>
        public T GetService<T>() where T : class
        {
            var provider = _provider;
            if (provider == null)
            {
                throw new InvalidOperationException("服务提供者尚未初始化，请先调用Initialize方法");
            }
            return provider.GetService<T>();
        }

        /// <summary>
        /// 获取指定类型的服务实例，未找到则抛出异常
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        public T GetKeyedService<T>(object obj) where T : class
        {
            var provider = _provider;
            if (provider == null)
            {
                throw new InvalidOperationException("服务提供者尚未初始化，请先调用Initialize方法");
            }
            var service = provider.GetKeyedService<T>(obj);
            if (service == null)
            {
                throw new InvalidOperationException($"未找到类型为{typeof(T).FullName}的服务注册");
            }
            return service;
        }

        /// <summary>
        /// 初始化服务定位器
        /// </summary>
        /// <param name="provider">服务提供者</param>
        public void Initialize(ITangdaoProvider provider)
        {
            TangdaoGuards.ThrowIfNull(provider, nameof(provider));
            if (Interlocked.CompareExchange(ref _provider, provider, null) != null)
            {
                throw new InvalidOperationException("服务定位器已经初始化，不能重复初始化");
            }
        }

        /// <summary>
        /// 服务提供者实例
        /// </summary>
        private volatile ITangdaoProvider _provider;
    }
}