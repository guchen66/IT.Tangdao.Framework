using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 任务添加项接口
    /// </summary>
    /// <remarks>
    /// 用于任务进度报告，传递任务添加的相关信息
    /// </remarks>
    public interface IAddTaskItem
    {
        /// <summary>
        /// 获取或设置新添加的任务项
        /// </summary>
        string NewItem { get; set; }
    }
}