using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// Tangdao 任务等待器
    /// 实现 ICriticalNotifyCompletion 接口，提供高效的异步等待能力
    /// 用于自定义任务的 await 模式支持
    /// </summary>
    /// <typeparam name="TResult">任务结果类型</typeparam>
    public readonly struct TangdaoTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        /// <summary>
        /// 内部封装的 TaskAwaiter
        /// </summary>
        private readonly TaskAwaiter<TResult> _awaiter;

        /// <summary>
        /// 初始化 TangdaoTaskAwaiter 实例
        /// </summary>
        /// <param name="task">要等待的任务</param>
        /// <param name="continueOnCapturedContext">是否在捕获的上下文中继续执行</param>
        public TangdaoTaskAwaiter(Task<TResult> task, bool continueOnCapturedContext)
        {
            _awaiter = task.GetAwaiter();
        }

        /// <summary>
        /// 获取一个值，该值指示异步任务是否已完成
        /// </summary>
        public bool IsCompleted => _awaiter.IsCompleted;

        /// <summary>
        /// 获取异步任务的结果
        /// 如果任务未完成或失败，将抛出相应异常
        /// </summary>
        /// <returns>任务的结果</returns>
        public TResult GetResult() => _awaiter.GetResult();

        /// <summary>
        /// 安排在异步任务完成时执行的延续操作
        /// </summary>
        /// <param name="continuation">异步任务完成后要调用的操作</param>
        public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

        /// <summary>
        /// 安排在异步任务完成时执行的延续操作（不捕获同步上下文）
        /// 此方法比 OnCompleted 更高效，但需要调用者确保线程安全性
        /// </summary>
        /// <param name="continuation">异步任务完成后要调用的操作</param>
        public void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);
    }
}