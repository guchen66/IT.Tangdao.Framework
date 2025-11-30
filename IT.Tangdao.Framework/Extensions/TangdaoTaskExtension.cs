using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.DaoTasks;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 任务扩展方法
    /// </summary>
    public static class TangdaoTaskExtension
    {
        /// <summary>
        /// 获取格式化的任务执行时间
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <returns>格式化后的任务执行时间，格式为hh:mm:ss.fff</returns>
        public static string Duration(this TangdaoTask task)
        {
            if (task == null)
            {
                TangdaoGuards.ThrowIfNull(nameof(task));
            }

            return task.Elapsed.ToString(@"hh\:mm\:ss\.fff");
        }

        /// <summary>
        /// 获取格式化的异步任务执行时间
        /// </summary>
        /// <param name="task">异步任务对象</param>
        /// <returns>格式化后的任务执行时间，格式为hh:mm:ss.fff</returns>
        public static string Duration(this TangdaoTaskAsync task)
        {
            if (task == null)
            {
                TangdaoGuards.ThrowIfNull(nameof(task));
            }

            return task.Elapsed.ToString(@"hh\:mm\:ss\.fff");
        }
    }
}