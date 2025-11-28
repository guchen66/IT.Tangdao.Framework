using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务添加项实现
    /// </summary>
    /// <remarks>
    /// 用于任务进度报告，传递任务添加的相关信息
    /// </remarks>
    public class AddTaskItem : IAddTaskItem
    {
        /// <summary>
        /// 获取或设置新添加的任务项
        /// </summary>
        public string NewItem { get; set; }
    }
}