using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.DaoEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.IServices;
using IT.Tangdao.Framework.Abstractions.Services;
using IT.Tangdao.Framework.Abstractions.Navigates;

namespace IT.Tangdao.Framework
{
    internal class TangdaoContainerBuilder : ITangdaoContainerBuilder
    {
        // 使用 Lazy<T> 实现单例模式
        private static readonly Lazy<ITangdaoProvider> _lazyProvider = new Lazy<ITangdaoProvider>(() => new TangdaoProvider());

        private static readonly Lazy<ITangdaoContainer> _lazyContainer = new Lazy<ITangdaoContainer>(() =>
        {
            var container = new TangdaoContainer();
            container.Register<ITangdaoProvider, TangdaoProvider>();
            container.Register<IDaoEventAggregator, DaoEventAggregator>();
            container.Register<IReadService, ReadService>();
            container.Register<IWriteService, WriteService>();
            container.Register<ITangdaoParameter, TangdaoParameter>();
            container.Register<ITangdaoRouter, TangdaoRouter>();
            container.Register<IMonitorService, FileMonitorService>();
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

        /// <summary>
        /// 创建一个ITangdaoContainer单例
        /// </summary>
        /// <returns></returns>
        public static ITangdaoContainer CreateContainer()
        {
            return _lazyContainer.Value;
        }
    }
}