using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Results;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务项实现类
    /// 表示一个可执行的任务单元，包含任务的所有状态信息和执行逻辑
    /// </summary>
    internal class TaskItem : ITaskItem
    {
        /// <summary>
        /// 获取任务的唯一标识符
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 获取任务的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取任务的优先级
        /// </summary>
        public TaskPriority Priority { get; }

        /// <summary>
        /// 获取任务的创建时间
        /// </summary>
        public DateTime CreatedTime { get; }

        /// <summary>
        /// 获取或设置任务的开始时间
        /// 任务开始执行时设置，未开始时为 null
        /// </summary>
        public DateTime? StartTime { get; internal set; }

        /// <summary>
        /// 获取或设置任务的完成时间
        /// 任务完成（成功、失败或取消）时设置，未完成时为 null
        /// </summary>
        public DateTime? CompletedTime { get; internal set; }

        /// <summary>
        /// 获取或设置任务的当前状态
        /// 默认值为 Created（已创建）
        /// </summary>
        public TaskStatus Status { get; internal set; } = TaskStatus.Created;

        /// <summary>
        /// 获取或设置任务的执行结果
        /// 任务完成后包含执行结果信息
        /// </summary>
        public IResponseResult<ITaskItem> Result { get; internal set; }

        /// <summary>
        /// 获取或设置任务完成源
        /// 用于异步等待任务完成，可通过 TCS.Task 等待任务结果
        /// </summary>
        public TaskCompletionSource<ITaskItem> TCS { get; internal set; }

        /// <summary>
        /// 获取或设置任务执行过程中发生的异常
        /// 仅当任务状态为 Faulted 时有值
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// 获取任务的执行委托
        /// 接受一个 CancellationToken 参数，返回 Task<object>
        /// </summary>
        public Func<CancellationToken, Task<object>> Action { get; }

        /// <summary>
        /// 获取任务的执行线程类型
        /// </summary>
        public TaskThreadType ThreadType { get; }

        /// <summary>
        /// 初始化 TaskItem 实例
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="priority">任务优先级</param>
        /// <param name="action">任务执行委托</param>
        public TaskItem(string name, TaskPriority priority, Func<CancellationToken, Task<object>> action) : this(name, priority, action, TaskThreadType.Auto)
        {
        }

        /// <summary>
        /// 初始化 TaskItem 实例
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="priority">任务优先级</param>
        /// <param name="action">任务执行委托</param>
        /// <param name="threadType">任务执行线程类型</param>
        public TaskItem(string name, TaskPriority priority, Func<CancellationToken, Task<object>> action, TaskThreadType threadType)
        {
            Id = Guid.NewGuid();
            Name = name;
            Priority = priority;
            Action = action;
            ThreadType = threadType;
            CreatedTime = DateTime.Now;
            TCS = new TaskCompletionSource<ITaskItem>();
        }
    }
}