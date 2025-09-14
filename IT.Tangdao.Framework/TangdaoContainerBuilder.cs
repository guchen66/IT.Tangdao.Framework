using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.DaoEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public class TangdaoContainerBuilder //: ITangdaoContainerBuilder
    {
        //private static ITangdaoProvider _singletonProvider;

        //private static ITangdaoContainer _singletonContainer;

        //private static readonly object providerLock = new object();

        //private static readonly object containerLock = new object();

        //public static ITangdaoProvider Builder()
        //{
        //    // 使用双重检查锁定（Double-Checked Locking）确保线程安全
        //    if (_singletonProvider == null)
        //    {
        //        lock (providerLock)
        //        {
        //            if (_singletonProvider == null)
        //            {
        //                _singletonProvider = new TangdaoProvider();
        //            }
        //        }
        //    }

        //    return _singletonProvider;
        //}

        //public static ITangdaoContainer CreateContainer()
        //{
        //    // 使用双重检查锁定（Double-Checked Locking）确保线程安全
        //    if (_singletonContainer == null)
        //    {
        //        lock (containerLock)
        //        {
        //            if (_singletonContainer == null)
        //            {
        //                _singletonContainer = new TangdaoContainer();
        //                _singletonContainer.Register<ITangdaoProvider, TangdaoProvider>();
        //            }
        //        }
        //    }

        //    return _singletonContainer;
        //}

        // 使用 Lazy<T> 实现单例模式
        private static readonly Lazy<ITangdaoProvider> _lazyProvider = new Lazy<ITangdaoProvider>(() => new TangdaoProvider());

        private static readonly Lazy<ITangdaoContainer> _lazyContainer = new Lazy<ITangdaoContainer>(() =>
        {
            var container = new TangdaoContainer();
            container.Register<ITangdaoProvider, TangdaoProvider>();
            return container;
        });

        /// <summary>
        /// 获取或创建一个 ITangdaoProvider 的单例实例。
        /// </summary>
        /// <returns>ITangdaoProvider 实例。</returns>
        public static ITangdaoProvider Builder()
        {
            return _lazyProvider.Value;
        }

        public static ITangdaoContainer CreateContainer()
        {
            return _lazyContainer.Value;
        }
    }

    public class TangdaoContainerBuilder<TService> where TService : class
    {
        public void Singleton()
        {
            // 获取单例实例
            TangdaoContext.GetInstance<TService>(TangdaoContainerBuilder.Builder());
        }
    }
}