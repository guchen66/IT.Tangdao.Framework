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
using IT.Tangdao.Framework.Abstractions.Messaging;
using IT.Tangdao.Framework.Abstractions.Navigation;
using IT.Tangdao.Framework.Commands;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.DaoTasks;
using IT.Tangdao.Framework.Windows;

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
            container.RegisterSingleton<IContentAccess, ContentAccess>();

            //注册读取地址服务
            container.RegisterSingleton<IFileLocator, FileLocator>();

            //注册发布通知服务
            container.RegisterSingleton<ITangdaoPublisher, TangdaoPublisher>();
            container.RegisterSingleton<ITangdaoNotifier, TangdaoNotifier>();

            //注册委托传输服务
            container.RegisterSingleton<IActionTable, ActionTable>();

            //注册事件聚合器
            container.RegisterSingleton<IEventAggregator, EventAggregator>();

            //注册异步任务流
            container.RegisterSingleton<ITaskQueueManager, TaskQueueManager>();

            //注册异步任务器
            container.RegisterSingleton<ITaskController, TaskController>();

            //登录窗体以及Window管理通道
            container.RegisterSingleton<IWindowBuilder, WindowBuilder>();
            container.RegisterTransient<IWindowPipeline, WindowPipeline>();
            container.RegisterTransient<IWindowGuard, LoginSignGuard>();
            container.RegisterSingleton<WindowAction>();

            //注册导航服务
            container.RegisterTransientFactory<ITangdaoRouterResolver>(provider =>
            {
                return new TangdaoRouterResolver(entry => provider.GetService(entry.RegisterType) as ITangdaoPage);
            });

            container.RegisterSingleton<ITangdaoRouter, TangdaoRouter>();
            var loader = new TangdaoConfigLoader();
            // 2. 立即 Load 并塞进容器
            container.RegisterSingleton(loader.Load());
        }
    }
}