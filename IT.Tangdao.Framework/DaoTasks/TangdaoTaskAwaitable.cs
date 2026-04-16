using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// Tangdao 可等待任务包装器
    /// 提供自定义的 await 模式支持，允许配置是否捕获同步上下文
    /// 用于构建流畅的异步编程体验
    /// </summary>
    /// <typeparam name="TResult">任务结果类型</typeparam>
    public readonly struct TangdaoTaskAwaitable<TResult>
    {
        /// <summary>
        /// 内部封装的任务
        /// </summary>
        private readonly Task<TResult> _task;

        /// <summary>
        /// 是否在捕获的同步上下文中继续执行
        /// </summary>
        private readonly bool _continueOnCapturedContext;

        /// <summary>
        /// 初始化 TangdaoTaskAwaitable 实例
        /// </summary>
        /// <param name="task">要包装的任务</param>
        /// <param name="continueOnCapturedContext">是否在捕获的上下文中继续执行，默认为 true</param>
        /// <exception cref="ArgumentNullException">当 task 为 null 时抛出</exception>
        public TangdaoTaskAwaitable(Task<TResult> task, bool continueOnCapturedContext = true)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        /// <summary>
        /// 获取用于等待此任务的 awaiter
        /// 这是 await 关键字调用的方法
        /// </summary>
        /// <returns>TangdaoTaskAwaiter 实例</returns>
        public TangdaoTaskAwaiter<TResult> GetAwaiter()
        {
            return new TangdaoTaskAwaiter<TResult>(_task, _continueOnCapturedContext);
        }
    }
}