using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务项实现
    /// </summary>
    internal class TaskItem : ITaskItem
    {
        public Guid Id { get; }
        public string Name { get; }
        public TaskPriority Priority { get; }
        public DateTime CreatedTime { get; }
        public DateTime? StartTime { get; internal set; }
        public DateTime? CompletedTime { get; internal set; }
        public TaskStatus Status { get; internal set; } = TaskStatus.Created;
        public object Result { get; internal set; }
        public Exception Exception { get; internal set; }

        // 任务执行委托
        public Func<CancellationToken, Task<object>> Action { get; }

        public TaskItem(string name, TaskPriority priority, Func<CancellationToken, Task<object>> action)
        {
            Id = Guid.NewGuid();
            Name = name;
            Priority = priority;
            Action = action;
            CreatedTime = DateTime.Now;
        }
    }
}