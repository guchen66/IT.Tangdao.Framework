using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// Tangdao任务调度器
    /// </summary>
    public static class TangdaoTaskScheduler
    {
        #region --------- 公共入口 ---------

        /// <summary>
        /// UI 线程同步执行（无 token，不可取消）
        /// </summary>
        public static void Execute(Action<TangdaoTask> dao)
        {
            if (dao == null) throw new ArgumentNullException(nameof(dao));
            TangdaoTask task = null;
            try
            {
                task = new TangdaoTask();
                UI(() => dao(task));          // 同步 Invoke，执行完才退出
                task.MarkCompleted();
            }
            catch (Exception ex)
            {
                task?.MarkFaulted(ex);
                throw;
            }
            finally
            {
                task?.Dispose();              // lambda 执行完才释放
            }
        }

        /// <summary>
        /// UI 线程异步执行（不可取消）
        /// </summary>
        public static void ExecuteAsync(Action<TangdaoTask> dao)
        {
            if (dao == null) throw new ArgumentNullException(nameof(dao));
            TangdaoTask task = null;
            try
            {
                task = new TangdaoTask();
                UIAsync(() => dao(task));          // 同步 Invoke，执行完才退出
                task.MarkCompleted();
            }
            catch (Exception ex)
            {
                task?.MarkFaulted(ex);
                throw;
            }
            finally
            {
                task?.Dispose();              // lambda 执行完才释放
            }
        }

        /// <summary>
        /// 后台线程执行（可取消）
        /// </summary>
        public static void Execute(Action<TangdaoTaskAsync> daoAsync, CancellationToken token = default)
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
                    task.MarkCompleted();
                }
                catch (Exception ex)
                {
                    task.MarkFaulted(ex);
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
        public static void Execute(Action<TangdaoTaskAsync> daoAsync, Action<TangdaoTask> dao, CancellationToken token = default)
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
                        asyncTask.MarkCompleted();
                    }, token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    asyncTask?.MarkFaulted(new TaskCanceledException("后台任务被取消"));
                    throw;
                }
                catch (Exception ex)
                {
                    asyncTask?.MarkFaulted(ex);
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
                        uiTask.MarkCompleted();
                    });
                }
                catch (Exception ex)
                {
                    uiTask?.MarkFaulted(ex);
                    throw;
                }
                finally
                {
                    uiTask?.Dispose();
                }
            }, token);
        }

        #endregion --------- 公共入口 ---------

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

        #endregion --------- 私有调度 ---------
    }
}