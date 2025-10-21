using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 给 ITangdaoProvider 加常用便捷方法。
    /// </summary>
    public static class TangdaoProviderExtension
    {
        public static T GetRequiredService<T>(this ITangdaoProvider provider) where T : class
        {
            return provider.GetService<T>() ?? throw new InvalidOperationException($"服务 '{typeof(T).Name}' 未注册。");
        }

        public static T GetRequiredService<T>(this ITangdaoProvider provider, Type serviceType)
            => (T)(provider.GetService(serviceType) ?? throw new InvalidOperationException($"服务 '{serviceType.Name}' 未注册。"));

        /// <summary>
        /// 按 key 获取服务；找不到返回 null。
        /// </summary>
        public static T GetKeyedService<T>(this ITangdaoProvider provider, object key) where T : class
            => (provider as TangdaoProvider)?.GetKeyedService<T>(key);

        /// <summary>
        /// 按 key 获取服务；找不到抛异常。
        /// </summary>
        public static T GetRequiredKeyedService<T>(this ITangdaoProvider provider, object key) where T : class
            => provider.GetKeyedService<T>(key)
               ?? throw new InvalidOperationException($"服务 '{typeof(T).Name}' (key='{key}') 未注册。");
    }
}