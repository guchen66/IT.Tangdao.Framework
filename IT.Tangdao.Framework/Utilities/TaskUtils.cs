using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Results;

namespace IT.Tangdao.Framework.Utilities
{
    /// <summary>
    /// 任务线程调度工具
    /// </summary>
    internal static class TaskUtils
    {
        /// <summary>
        /// 获取UI线程的Dispatcher
        /// </summary>
        private static Dispatcher UIDispatcher => Application.Current?.Dispatcher
            ?? throw new InvalidOperationException("WPF Application is not running.");

        /// <summary>
        /// 根据线程类型执行同步操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="threadType">线程类型</param>
        /// <returns>执行结果</returns>
        public static ResponseResult ExecuteWithThreadType(Action action, TaskThreadType threadType)
        {
            try
            {
                switch (threadType)
                {
                    case TaskThreadType.UI:
                        ExecuteUITask(action);
                        break;

                    case TaskThreadType.Background:
                        ExecuteBackgroundTask(action);
                        break;

                    case TaskThreadType.UIIdle:
                        ExecuteUIIdleTask(action);
                        break;

                    case TaskThreadType.Auto:
                    default:
                        ExecuteAutoTask(action);
                        break;
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.FromException(ex, $"线程类型 {threadType} 执行失败");
            }
        }

        /// <summary>
        /// 在UI线程执行（同步等待）
        /// </summary>
        private static void ExecuteUITask(Action action)
        {
            if (UIDispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                UIDispatcher.Invoke(action);
            }
        }

        /// <summary>
        /// 在后台线程执行（不等待，Fire-and-Forget）
        /// </summary>
        private static void ExecuteBackgroundTask(Action action)
        {
            Task.Run(action);
        }

        /// <summary>
        /// 在UI空闲时执行（异步，不等待）
        /// </summary>
        private static void ExecuteUIIdleTask(Action action)
        {
            UIDispatcher.InvokeAsync(action, DispatcherPriority.SystemIdle);
        }

        /// <summary>
        /// 自动选择线程类型
        /// </summary>
        private static void ExecuteAutoTask(Action action)
        {
            if (UIDispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                UIDispatcher.InvokeAsync(action, DispatcherPriority.SystemIdle);
            }
        }

        /// <summary>
        /// 根据线程类型执行异步操作
        /// </summary>
        /// <param name="action">要执行的异步操作</param>
        /// <param name="threadType">线程类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>执行结果</returns>
        public static async Task<ResponseResult> ExecuteWithThreadTypeAsync(Func<Task> action, TaskThreadType threadType, CancellationToken cancellationToken = default)
        {
            try
            {
                switch (threadType)
                {
                    case TaskThreadType.UI:
                        await ExecuteUITaskAsync(action, cancellationToken);
                        break;

                    case TaskThreadType.Background:
                        await Task.Run(action, cancellationToken);
                        break;

                    case TaskThreadType.UIIdle:
                        await ExecuteUIIdleTaskAsync(action);
                        break;

                    case TaskThreadType.Auto:
                    default:
                        await ExecuteAutoTaskAsync(action, cancellationToken);
                        break;
                }
                return ResponseResult.Success();
            }
            catch (OperationCanceledException)
            {
                return ResponseResult.Failure("任务被取消");
            }
            catch (Exception ex)
            {
                return ResponseResult.FromException(ex, $"线程类型 {threadType} 执行失败");
            }
        }

        /// <summary>
        /// 在UI线程执行异步操作
        /// </summary>
        private static Task ExecuteUITaskAsync(Func<Task> action, CancellationToken cancellationToken)
        {
            if (UIDispatcher.CheckAccess())
            {
                return action();
            }
            return UIDispatcher.InvokeAsync(action, DispatcherPriority.Normal, cancellationToken).Task.Unwrap();
        }

        /// <summary>
        /// 在UI空闲时执行异步操作
        /// </summary>
        private static async Task ExecuteUIIdleTaskAsync(Func<Task> action)
        {
            var tcs = new TaskCompletionSource<bool>();

            _ = UIDispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    await action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), DispatcherPriority.SystemIdle);

            await tcs.Task;
        }

        /// <summary>
        /// 自动选择线程类型执行异步操作
        /// </summary>
        private static async Task ExecuteAutoTaskAsync(Func<Task> action, CancellationToken cancellationToken)
        {
            if (UIDispatcher.CheckAccess())
            {
                await action();
            }
            else
            {
                await UIDispatcher.InvokeAsync(action, DispatcherPriority.Normal, cancellationToken).Task.Unwrap();
            }
        }
    }
}