using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    public interface ITangdaoTaskService<T>
    {
        Task StartAsync(IProgress<T> progress);

        void Pause();

        void Resume();

        void Stop();
    }
}