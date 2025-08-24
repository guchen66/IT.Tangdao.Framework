using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework
{
    public static class TangdaoTaskScheduler
    {
        // 重载1：只执行UI任务（同步）
        public static void Execute(Action<TangdaoTask> dao)
        {
            if (dao == null) throw new ArgumentNullException(nameof(dao));

            var task = new TangdaoTask();

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    dao(task); // 直接在UI线程执行Lambda中的代码
                });
            }
        }

        // 重载2：只执行异步任务
        public static void Execute(Action<TangdaoTaskAsync> daoAsync)
        {
            if (daoAsync == null) throw new ArgumentNullException(nameof(daoAsync));

            var asyncTask = new TangdaoTaskAsync();

            // 在后台线程执行Lambda中的代码
            Task.Run(() =>
            {
                daoAsync(asyncTask); // 直接在后台线程执行
            });
        }

        // 重载3：先执行异步，再执行UI
        public static void Execute(Action<TangdaoTaskAsync> daoAsync, Action<TangdaoTask> dao)
        {
            if (daoAsync == null) throw new ArgumentNullException(nameof(daoAsync));
            if (dao == null) throw new ArgumentNullException(nameof(dao));

            var asyncTask = new TangdaoTaskAsync();
            var uiTask = new TangdaoTask();

            // 先在后台线程执行异步任务
            Task.Run(() =>
            {
                daoAsync(asyncTask); // 执行异步代码

                // 完成后回到UI线程执行UI任务
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        dao(uiTask); // 执行UI代码
                    });
                }
            });
        }
    }
}