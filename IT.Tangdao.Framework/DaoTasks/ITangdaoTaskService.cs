using IT.Tangdao.Framework.Parameters.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    public interface ITangdaoTaskService
    {
        Task StartAsync(IProgress<IAddTaskItem> progress);

        void Pause();

        void Resume();

        void Stop();
    }
}