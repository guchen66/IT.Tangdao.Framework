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

namespace IT.Tangdao.Framework
{
    public abstract class TangdaoApplicationBase : Application, IAppHost<TangdaoHost>, ITangdaoDataProvider
    {
        public TangdaoPipe<TangdaoHost> Handler { get; set; }

        protected virtual void RegisterServices(ITangdaoContainer container)
        {
        }

        protected virtual TangdaoContainerBuilder CreateContainer()
        {
            return new TangdaoContainerBuilder();
        }

        public virtual async Task AsyncTaskHandler(ITaskQueueManager taskQueueManager)
        {
            await taskQueueManager.Empty();
        }

        public virtual void ConfigureWindowPipe(IWindowBuilder windowBuilder)
        {
        }
    }
}