using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务项接口
    /// </summary>
    public interface ITaskItem
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 任务名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 任务状态
        /// </summary>
        TaskStatus Status { get; }

        /// <summary>
        /// 任务优先级
        /// </summary>
        TaskPriority Priority { get; }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        DateTime CreatedTime { get; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        /// 任务完成时间
        /// </summary>
        DateTime? CompletedTime { get; }

        /// <summary>
        /// 任务执行结果
        /// </summary>
        object Result { get; }

        /// <summary>
        /// 任务异常信息
        /// </summary>
        Exception Exception { get; }
    }
}