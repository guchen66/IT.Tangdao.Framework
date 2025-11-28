using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 进度报告接口
    /// </summary>
    /// <remarks>
    /// 用于报告任务执行进度，支持百分比进度、状态文本和取消操作
    /// 继承自.NET内置的IProgress接口，实现更好的框架集成
    /// </remarks>
    public interface IProgressReport : IProgress<TangdaoProgressChangedEventArgs>
    {
        /// <summary>
        /// 获取或设置任务总工作量
        /// </summary>
        long Total { get; set; }

        /// <summary>
        /// 获取或设置已完成工作量
        /// </summary>
        long Completed { get; set; }

        /// <summary>
        /// 获取或设置当前状态文本
        /// </summary>
        string StatusText { get; set; }

        /// <summary>
        /// 获取当前进度百分比
        /// </summary>
        double ProgressPercentage { get; }

        /// <summary>
        /// 获取或设置是否已取消
        /// </summary>
        bool IsCancelled { get; set; }

        /// <summary>
        /// 报告进度更新
        /// </summary>
        /// <param name="completed">已完成工作量</param>
        /// <param name="total">总工作量</param>
        /// <param name="statusText">状态文本</param>
        void Report(long completed, long total, string statusText = null);

        /// <summary>
        /// 报告进度更新
        /// </summary>
        /// <param name="progressPercentage">进度百分比（0-100）</param>
        /// <param name="statusText">状态文本</param>
        void Report(double progressPercentage, string statusText = null);

        /// <summary>
        /// 取消任务
        /// </summary>
        void Cancel();

        /// <summary>
        /// 检查是否已取消，如果已取消则抛出异常
        /// </summary>
        /// <exception cref="OperationCanceledException">如果已取消</exception>
        void ThrowIfCancellationRequested();

        /// <summary>
        /// 进度更新事件
        /// </summary>
        event EventHandler<TangdaoProgressChangedEventArgs> ProgressChanged;
    }
}