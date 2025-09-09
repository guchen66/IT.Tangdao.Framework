using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    public class WheelTask<T>
    {
        public T Data { get; set; }          // 任务携带的数据
        public Func<T, Task> Handler { get; set; } // 任务处理函数（异步）

        public WheelTask(T data, Func<T, Task> handler)
        {
            this.Data = data;
            this.Handler = handler;
        }
    }
}