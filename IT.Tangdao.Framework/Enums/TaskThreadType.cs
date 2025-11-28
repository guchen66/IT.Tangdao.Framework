using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 任务执行线程类型
    /// </summary>
    public enum TaskThreadType
    {
        /// <summary>
        /// 自动选择线程类型
        /// <remarks>
        /// 如果当前在UI线程，则使用UI线程；否则使用后台线程
        /// </remarks>
        /// </summary>
        Auto = 0,

        /// <summary>
        /// UI线程
        /// <remarks>
        /// 确保任务在UI线程执行，用于更新UI组件
        /// </remarks>
        /// </summary>
        UI = 1,

        /// <summary>
        /// 后台线程
        /// <remarks>
        /// 确保任务在后台线程执行，用于执行耗时操作
        /// </remarks>
        /// </summary>
        Background = 2,

        /// <summary>
        /// UI线程空闲时执行
        /// <remarks>
        /// 当UI线程空闲时执行任务，用于执行低优先级操作
        /// </remarks>
        /// </summary>
        UIIdle = 3
    }
}