using IT.Tangdao.Framework.Bootstrap;
using IT.Tangdao.Framework.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.DaoTasks;
using System.Reflection;
using IT.Tangdao.Framework.Windows;
using IT.Tangdao.Framework.Abstractions.Contracts;

namespace IT.Tangdao.Framework
{
    public abstract class TangdaoApplicationBase : Application, IAppHost<ITangdaoHost>, ITangdaoDataProvider
    {
        public TangdaoPipe<ITangdaoHost> Handler { get; set; }

        public IBindHandler Binding { get; } = new BindHandler();

        /// <summary>
        /// 容器注册
        /// </summary>
        /// <param name="container"></param>
        protected virtual void RegisterServices(ITangdaoContainer container)
        {
        }

        protected virtual TangdaoContainerBuilder CreateContainer()
        {
            return new TangdaoContainerBuilder();
        }

        /// <summary>
        /// 异步任务调度器
        /// </summary>
        /// <param name="taskQueueManager"></param>
        /// <returns></returns>
        public virtual async Task AsyncTaskHandler(ITaskQueueManager taskQueueManager)
        {
            await taskQueueManager.Empty();
        }

        /// <summary>
        /// 配置
        /// </summary>
        protected virtual void Configure()
        {
        }

        /// <summary>
        /// 窗体通道
        /// </summary>
        /// <param name="windowBuilder"></param>
        public virtual void ConfigureWindowPipe(IWindowBuilder windowBuilder)
        {
        }

        /// <summary>
        /// 创建宿主数据
        /// </summary>
        /// <returns></returns>
        public virtual ITangdaoHost CreateHost()
        {
            return new TangdaoHost();
        }
    }
}