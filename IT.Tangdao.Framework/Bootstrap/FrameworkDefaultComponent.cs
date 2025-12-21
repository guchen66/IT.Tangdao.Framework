using IT.Tangdao.Framework.Component;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.FileAccessor;
using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Notices;
using IT.Tangdao.Framework.Abstractions.Navigation;
using IT.Tangdao.Framework.Commands;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Bootstrap
{
    /// <summary>
    /// 框架默认注册服务
    /// </summary>
    internal sealed class FrameworkDefaultComponent : ITangdaoContainerComponent
    {
        public void Load(ITangdaoContainer container, TangdaoComponentContext context)
        {
            //注册读写服务
            container.AddTangdaoSingleton<IContentReader, ContentReader>();
            container.AddTangdaoSingleton<IContentWriter, ContentWriter>();

            //注册读取地址服务
            container.AddTangdaoSingleton<IFileLocator, FileLocator>();

            //注册发布通知服务
            container.AddTangdaoSingleton<ITangdaoPublisher, TangdaoPublisher>();
            container.AddTangdaoSingleton<ITangdaoNotifier, TangdaoNotifier>();

            //注册委托传输服务
            container.AddTangdaoSingleton<IHandlerTable, HandlerTable>();

            //注册事件聚合器
            container.AddTangdaoSingleton<IDaoEventAggregator, DaoEventAggregator>();

            //注册导航服务

            //container.AddTangdaoSingleton<RegistrationTypeEntry>();
            //container.AddTangdaoSingleton<ITangdaoRouter, TangdaoRouter>();
            //container.AddTangdaoSingletonFactory<ITangdaoRouterResolver>(provider =>
            //{
            //    var Resolver = provider.GetService<ITangdaoRouterResolver>();
            //    return new TangdaoRouterResolver(entry => provider.GetService(entry.RegisterType) as ITangdaoPage);
            //});
            var loader = new TangdaoConfigLoader();
            // 2. 立即 Load 并塞进容器
            container.AddTangdaoSingleton(loader.Load());
        }
    }
}