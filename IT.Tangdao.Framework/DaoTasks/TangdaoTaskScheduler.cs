using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// Tangdao任务调度器
    /// </summary>
    public static class TangdaoTaskScheduler
    {
        #region --------- 公共入口 ---------

        /// <summary>
        /// 执行任务（无 token，不可取消）
        /// </summary>
        /// <param name="dao">任务执行委托</param>
        /// <param name="threadType">任务执行线程类型</param>
        public static void Execute(Action<TangdaoTask> dao, TaskThreadType threadType = TaskThreadType.Auto)
        {
            if (dao == null) throw new ArgumentNullException(nameof(dao));
            TangdaoTask task = null;
            try
            {
                task = new TangdaoTask();
                ExecuteWithThreadType(() => dao(task), threadType);
                task.OnCompleted();
            }
            catch (Exception ex)
            {
                task?.OnFaulted(ex);
                throw;
            }
            finally
            {
                task?.Dispose();
            }
        }

        /// <summary>
        /// 异步执行任务（不可取消）
        /// </summary>
        /// <param name="dao">任务执行委托</param>
        /// <param name="threadType">任务执行线程类型</param>
        public static void ExecuteAsync(Action<TangdaoTask> dao, TaskThreadType threadType = TaskThreadType.Auto)
        {
            if (dao == null) throw new ArgumentNullException(nameof(dao));
            TangdaoTask task = null;
            try
            {
                task = new TangdaoTask();
                ExecuteAsyncWithThreadType(() => dao(task), threadType);
                task.OnCompleted();
            }
            catch (Exception ex)
            {
                task?.OnFaulted(ex);
                throw;
            }
            finally
            {
                task?.Dispose();
            }
        }

        /// <summary>
        /// 异步执行任务并返回结果
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="dao">任务执行委托</param>
        /// <param name="threadType">任务执行线程类型</param>
        /// <returns>任务执行结果</returns>
        public static async Task<TResult> ExecuteAsync<TResult>(Func<TangdaoTask, Task<TResult>> dao, TaskThreadType threadType = TaskThreadType.Auto)
        {
            if (dao == null) throw new ArgumentNullException(nameof(dao));
            TangdaoTask task = null;
            try
            {
                task = new TangdaoTask();
                TResult result = await ExecuteAsyncWithThreadType(async () => await dao(task), threadType);
                task.OnCompleted();
                return result;
            }
            catch (Exception ex)
            {
                task?.OnFaulted(ex);
                throw;
            }
            finally
            {
                task?.Dispose();
            }
        }

        /// <summary>
        /// 根据线程类型执行同步操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="threadType">线程类型</param>
        private static void ExecuteWithThreadType(Action action, TaskThreadType threadType)
        {
            switch (threadType)
            {
                case TaskThreadType.UI:
                    // 在UI线程执行
                    UI(action);
                    break;

                case TaskThreadType.Background:
                    // 在后台线程执行
                    Task.Run(action).Wait();
                    break;

                case TaskThreadType.UIIdle:
                    // 在UI线程空闲时执行
                    UIAsync(action).Wait();
                    break;

                case TaskThreadType.Auto:
                default:
                    // 自动选择线程类型
                    if (Dispatcher.CheckAccess())
                    {
                        action();
                    }
                    else
                    {
                        UI(action);
                    }
                    break;
            }
        }

        /// <summary>
        /// 根据线程类型执行异步操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="threadType">线程类型</param>
        private static void ExecuteAsyncWithThreadType(Action action, TaskThreadType threadType)
        {
            switch (threadType)
            {
                case TaskThreadType.UI:
                    // 在UI线程异步执行
                    UIAsync(action);
                    break;

                case TaskThreadType.Background:
                    // 在后台线程异步执行
                    Task.Run(action);
                    break;

                case TaskThreadType.UIIdle:
                    // 在UI线程空闲时执行
                    UIAsync(action);
                    break;

                case TaskThreadType.Auto:
                default:
                    // 自动选择线程类型
                    if (Dispatcher.CheckAccess())
                    {
                        action();
                    }
                    else
                    {
                        UIAsync(action);
                    }
                    break;
            }
        }

        /// <summary>
        /// 根据线程类型执行异步操作并返回结果
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">要执行的函数</param>
        /// <param name="threadType">线程类型</param>
        /// <returns>任务执行结果</returns>
        private static async Task<TResult> ExecuteAsyncWithThreadType<TResult>(Func<Task<TResult>> func, TaskThreadType threadType)
        {
            switch (threadType)
            {
                case TaskThreadType.UI:
                    // 在UI线程异步执行
                    return await UIAsync(func);

                case TaskThreadType.Background:
                    // 在后台线程异步执行
                    return await Task.Run(func);

                case TaskThreadType.UIIdle:
                    // 在UI线程空闲时执行
                    return await UIAsync(func);

                case TaskThreadType.Auto:
                default:
                    // 自动选择线程类型
                    if (Dispatcher.CheckAccess())
                    {
                        return await func();
                    }
                    else
                    {
                        return await UIAsync(func);
                    }
            }
        }

        /// <summary>
        /// 后台线程执行（可取消）
        /// </summary>
        public static void ExecuteAsyncTask(Action<TangdaoTaskAsync> daoAsync, CancellationToken token = default)
        {
            if (daoAsync == null) throw new ArgumentNullException(nameof(daoAsync));
            _ = Task.Run(() =>
            {
                var task = new TangdaoTaskAsync
                {
                    CancellationToken = token   // 注入令牌
                };
                try
                {
                    daoAsync(task);
                    task.OnCompleted();
                }
                catch (Exception ex)
                {
                    task.OnFaulted(ex);
                    throw;
                }
                finally
                {
                    task.Dispose();
                }
            }, token);
        }

        /// <summary>
        /// 先后台再 UI（可取消后台阶段）
        /// </summary>
        public static void ExecuteBackgroundThenUI(Action<TangdaoTaskAsync> daoAsync, Action<TangdaoTask> dao, CancellationToken token = default)
        {
            if (daoAsync == null) throw new ArgumentNullException(nameof(daoAsync));
            if (dao == null) throw new ArgumentNullException(nameof(dao));
            _ = Task.Run(async () =>
            {
                TangdaoTaskAsync asyncTask = null;
                try
                {
                    // 第一阶段：后台异步任务
                    asyncTask = new TangdaoTaskAsync { CancellationToken = token };
                    await Task.Run(() =>
                    {
                        daoAsync(asyncTask);
                        asyncTask.OnCompleted();
                    }, token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    asyncTask?.OnFaulted(new TaskCanceledException("后台任务被取消"));
                    throw;
                }
                catch (Exception ex)
                {
                    asyncTask?.OnFaulted(ex);
                    throw;
                }
                finally
                {
                    asyncTask?.Dispose();
                }

                // 第二阶段：UI任务
                TangdaoTask uiTask = null;
                try
                {
                    await UIAsync(() =>
                    {
                        uiTask = new TangdaoTask();
                        dao(uiTask);
                        uiTask.OnCompleted();
                    });
                }
                catch (Exception ex)
                {
                    uiTask?.OnFaulted(ex);
                    throw;
                }
                finally
                {
                    uiTask?.Dispose();
                }
            }, token);
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        public static void Destruction()
        {
            // 销毁时间轮实例
            _timeWheel?.Dispose();
            _timeWheel = null;
        }

        #endregion --------- 公共入口 ---------

        #region --------- 时间轮调度 ---------

        // 单例时间轮实例
        private static TimeWheel<TimeWheelTaskData> _timeWheel;

        /// <summary>
        /// 时间轮任务数据
        /// </summary>
        private class TimeWheelTaskData
        {
            /// <summary>
            /// 任务ID
            /// </summary>
            public Guid TaskId { get; set; }

            /// <summary>
            /// 任务名称
            /// </summary>
            public string TaskName { get; set; }

            /// <summary>
            /// 任务执行委托
            /// </summary>
            public Func<CancellationToken, Task<ResponseResult>> TaskAction { get; set; }

            /// <summary>
            /// 任务完成回调
            /// </summary>
            public Action<Guid, ResponseResult> CompletionCallback { get; set; }

            /// <summary>
            /// 取消令牌源
            /// </summary>
            public CancellationTokenSource CancellationTokenSource { get; set; }
        }

        /// <summary>
        /// 初始化时间轮
        /// </summary>
        private static void InitializeTimeWheel()
        {
            if (_timeWheel == null)
            {
                _timeWheel = new TimeWheel<TimeWheelTaskData>();
                _timeWheel.Start();
            }
        }

        /// <summary>
        /// 在指定延迟后执行任务
        /// </summary>
        /// <param name="delaySeconds">延迟秒数</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="taskAction">任务执行委托</param>
        /// <param name="completionCallback">任务完成回调</param>
        /// <returns>任务ID</returns>
        public static Guid ScheduleTask(int delaySeconds, string taskName, Func<CancellationToken, Task<ResponseResult>> taskAction, Action<Guid, ResponseResult> completionCallback = null)
        {
            if (taskAction == null)
                throw new ArgumentNullException(nameof(taskAction));

            InitializeTimeWheel();

            var taskId = Guid.NewGuid();
            var cts = new CancellationTokenSource();

            var taskData = new TimeWheelTaskData
            {
                TaskId = taskId,
                TaskName = taskName,
                TaskAction = taskAction,
                CompletionCallback = completionCallback,
                CancellationTokenSource = cts
            };

            // 添加任务到时间轮
            _timeWheel.AddTaskAsync(delaySeconds, taskData, async (data) =>
            {
                ResponseResult result;

                try
                {
                    // 执行任务
                    result = await data.TaskAction(data.CancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    result = ResponseResult.Failure($"任务 '{data.TaskName}' 已取消");
                }
                catch (Exception ex)
                {
                    result = ResponseResult.FromException(ex, data.TaskName);
                }
                finally
                {
                    // 释放资源
                    data.CancellationTokenSource.Dispose();
                }

                // 调用回调
                data.CompletionCallback?.Invoke(data.TaskId, result);
            }).ConfigureAwait(false);

            return taskId;
        }

        /// <summary>
        /// 在指定延迟后执行任务并返回泛型结果
        /// </summary>
        /// <typeparam name="T">结果数据类型</typeparam>
        /// <param name="delaySeconds">延迟秒数</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="taskAction">任务执行委托</param>
        /// <param name="completionCallback">任务完成回调</param>
        /// <returns>任务ID</returns>
        public static Guid ScheduleTask<T>(int delaySeconds, string taskName, Func<CancellationToken, Task<ResponseResult<T>>> taskAction, Action<Guid, ResponseResult<T>> completionCallback = null)
        {
            if (taskAction == null)
                throw new ArgumentNullException(nameof(taskAction));

            InitializeTimeWheel();

            var taskId = Guid.NewGuid();
            var cts = new CancellationTokenSource();

            var taskData = new TimeWheelTaskData
            {
                TaskId = taskId,
                TaskName = taskName,
                TaskAction = async (token) => await taskAction(token),
                CompletionCallback = (id, result) =>
                {
                    if (completionCallback != null)
                    {
                        var genericResult = result as ResponseResult<T> ?? ResponseResult<T>.Failure(result.Message, result.Exception, result.Value);
                        completionCallback(id, genericResult);
                    }
                },
                CancellationTokenSource = cts
            };

            // 添加任务到时间轮
            _timeWheel.AddTaskAsync(delaySeconds, taskData, async (data) =>
            {
                ResponseResult result;

                try
                {
                    // 执行任务
                    result = await data.TaskAction(data.CancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    result = ResponseResult<T>.Failure($"任务 '{data.TaskName}' 已取消");
                }
                catch (Exception ex)
                {
                    result = ResponseResult<T>.FromException(ex, data.TaskName);
                }
                finally
                {
                    // 释放资源
                    data.CancellationTokenSource.Dispose();
                }

                // 调用回调
                data.CompletionCallback?.Invoke(data.TaskId, result);
            }).ConfigureAwait(false);

            return taskId;
        }

        /// <summary>
        /// 取消指定的定时任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否取消成功</returns>
        public static bool CancelScheduledTask(Guid taskId)
        {
            // 目前TimeWheel不支持直接取消任务，需要扩展TimeWheel实现
            // 这里返回false表示取消失败
            return false;
        }

        #endregion --------- 时间轮调度 ---------

        #region --------- 私有调度 ---------

        private static Dispatcher Dispatcher =>
            Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        private static void UI(Action action)
        {
            if (Dispatcher.CheckAccess()) action();
            else Dispatcher.Invoke(action);
        }

        private static Task UIAsync(Action action)
            => Dispatcher.CheckAccess()
               ? Task.CompletedTask
               : Dispatcher.InvokeAsync(action, DispatcherPriority.Normal).Task;

        private static Task<TResult> UIAsync<TResult>(Func<TResult> func)
            => Dispatcher.CheckAccess()
               ? Task.FromResult(func())
               : Dispatcher.InvokeAsync(func, DispatcherPriority.Normal).Task;

        private static Task<TResult> UIAsync<TResult>(Func<Task<TResult>> asyncFunc)
            => Dispatcher.CheckAccess()
               ? asyncFunc()
               : Dispatcher.InvokeAsync(asyncFunc, DispatcherPriority.Normal).Task.Unwrap();

        #endregion --------- 私有调度 ---------
    }
}