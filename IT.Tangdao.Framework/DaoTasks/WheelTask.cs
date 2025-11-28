using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 时间轮任务
    /// </summary>
    /// <typeparam name="T">任务数据类型</typeparam>
    /// <remarks>
    /// 用于时间轮调度的任务封装，包含任务数据和异步处理函数
    /// </remarks>
    public class WheelTask<T>
    {
        /// <summary>
        /// 获取或设置任务携带的数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 获取或设置任务处理函数（异步）
        /// </summary>
        public Func<T, Task> Handler { get; set; }

        /// <summary>
        /// 初始化WheelTask类的新实例
        /// </summary>
        /// <param name="data">任务携带的数据</param>
        /// <param name="handler">任务处理函数（异步）</param>
        public WheelTask(T data, Func<T, Task> handler)
        {
            this.Data = data;
            this.Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
    }
}