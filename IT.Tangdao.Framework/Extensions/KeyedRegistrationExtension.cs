using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Ioc;

namespace IT.Tangdao.Framework.Extensions
{
    public static class KeyedRegistrationExtension
    {
        #region 泛型便利

        public static ITangdaoContainer AddKeyedTransient<TService, TImpl>(this ITangdaoContainer container,
                                                                            object key)
            where TImpl : TService
            => AddKeyed<TService, TImpl>(container, key, new TransientStrategy());

        public static ITangdaoContainer AddKeyedSingleton<TService, TImpl>(this ITangdaoContainer container,
                                                                            object key)
            where TImpl : TService
            => AddKeyed<TService, TImpl>(container, key, new SingletonStrategy());

        public static ITangdaoContainer AddKeyedScoped<TService, TImpl>(this ITangdaoContainer container,
                                                                         object key)
            where TImpl : TService
            => AddKeyed<TService, TImpl>(container, key, new ScopedStrategy());

        #endregion 泛型便利

        #region 底层统一入口

        private static ITangdaoContainer AddKeyed<TService, TImpl>(ITangdaoContainer container,
                                                                   object key,
                                                                   ILifecycleStrategy strategy)
        {
            var entry = new KeyedServiceEntry(typeof(TService), typeof(TImpl), strategy, key);
            //  keyed 注册走新字典，不干扰普通注册
            (container.Registry as ServiceRegistry)?.AddKeyed(entry, key);
            return container;
        }

        #endregion 底层统一入口
    }
}