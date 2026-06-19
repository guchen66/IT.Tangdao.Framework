using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Utilities;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// Tangdao任务调度器
    /// </summary>
    public static class TangdaoTaskScheduler
    {
        /// <summary>
        /// 同步执行任务
        /// </summary>
        /// <param name="dao">任务执行委托</param>
        /// <param name="threadType">线程类型</param>
        /// <returns>任务对象</returns>
        public static TangdaoTask Execute(Action<TangdaoTask> dao, TaskThreadType threadType = TaskThreadType.Auto)
        {
            if (dao == null)
                throw new ArgumentNullException(nameof(dao));

            var task = new TangdaoTask();

            var result = TaskUtils.ExecuteWithThreadType(() => dao(task), threadType);

            return result.IsSuccess ? task.SetResult() : task.SetException(result.Exception);
        }

        /// <summary>
        /// 异步执行任务
        /// </summary>
        /// <param name="dao">任务执行委托</param>
        /// <param name="threadType">线程类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务对象</returns>
        public static async Task<TangdaoTask> ExecuteAsync(Func<TangdaoTask, Task> dao, TaskThreadType threadType = TaskThreadType.Auto, CancellationToken cancellationToken = default)
        {
            if (dao == null)
                throw new ArgumentNullException(nameof(dao));

            var task = new TangdaoTask { CancellationToken = cancellationToken };

            var result = await TaskUtils.ExecuteWithThreadTypeAsync(() => dao(task), threadType, cancellationToken);

            return result.IsSuccess ? task.SetResult() : task.SetException(result.Exception);
        }

        /// <summary>
        /// 时间轮调度执行（重复执行指定次数）
        /// </summary>
        /// <param name="dao">业务逻辑委托</param>
        /// <param name="executeCount">执行次数</param>
        /// <param name="intervalMilliseconds">每次执行间隔（毫秒）</param>
        /// <param name="threadType">线程类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务对象</returns>
        public static async Task<TangdaoTask> TimeWheelExecuteAsync(Func<TangdaoTask, Task> dao, int executeCount, int intervalMilliseconds = 1000, TaskThreadType threadType = TaskThreadType.Auto, CancellationToken cancellationToken = default)
        {
            if (dao == null)
                throw new ArgumentNullException(nameof(dao));
            if (executeCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(executeCount), "执行次数必须大于0");
            if (intervalMilliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(intervalMilliseconds), "间隔时间不能为负数");

            var task = new TangdaoTask { CancellationToken = cancellationToken };
            var executeIndex = 0;

            try
            {
                for (int i = 0; i < executeCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    executeIndex = i + 1;
                    var result = await TaskUtils.ExecuteWithThreadTypeAsync(
                        () => dao(task),
                        threadType,
                        cancellationToken
                    );

                    if (!result.IsSuccess)
                    {
                        task.SetException(result.Exception ?? new Exception(result.Message));
                        return task;
                    }

                    // 最后一次执行后不需要等待
                    if (i < executeCount - 1)
                    {
                        await Task.Delay(intervalMilliseconds, cancellationToken);
                    }
                }

                task.SetResult();
            }
            catch (OperationCanceledException)
            {
                task.SetCanceled();
            }
            catch (Exception ex)
            {
                task.SetException(ex);
            }

            return task;
        }
    }
}