using IT.Tangdao.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using IT.Tangdao.Framework.Abstractions;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoViewExtension
    {
        //public static Task RunParentWindowAsync<TWindow>(this DaoViewModelBase viewModel, ITangdaoParameter tangdaoParameter) where TWindow : Window, new()
        //{
        //    var uiSyncContext = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        //    var tc = new TaskCompletionSource<object>();

        //    uiSyncContext.Invoke(() =>
        //    {
        //        TWindow view = new TWindow();
        //        view.Closed += View_Closed;
        //        ITangdaoMessage tangdaoWin = view.DataContext as ITangdaoMessage;
        //        view.Loaded += (obj, e) =>
        //        {
        //            tangdaoWin.Response(tangdaoParameter);
        //        };

        //        view.Show();
        //    });

        //    return tc.Task;
        //}

        ///// <summary>
        ///// 从ViewModel直接打开窗体
        ///// 并且传递参数或者委托到一个平级的窗体
        ///// </summary>
        ///// <typeparam name="TWindow"></typeparam>
        ///// <param name="viewModel"></param>
        ///// <param name="tangdaoParameter"></param>
        ///// <returns></returns>
        //public static Task RunSameLevelWindowAsync<TWindow>(this DaoViewModelBase viewModel, ITangdaoParameter tangdaoParameter) where TWindow : Window, new()
        //{
        //    TaskCompletionSource<object> tc = new TaskCompletionSource<object>();

        //    Thread thread = new Thread(() =>
        //    {
        //        TWindow view = new TWindow();
        //        view.Closed += View_Closed;
        //        ITangdaoMessage tangdaoWin = view.DataContext as ITangdaoMessage;
        //        view.Loaded += (obj, e) =>
        //        {
        //            tangdaoWin.Response(tangdaoParameter);
        //        };
        //        view.Show();

        //        //如果不加这段代码，窗体因为没有启动消息循环，一闪而过
        //        System.Windows.Threading.Dispatcher.Run();

        //        tc.SetResult(null);
        //    });
        //    thread.SetApartmentState(ApartmentState.STA);
        //    thread.Start();

        //    //新线程启动，将Task实例返回，接收await操作符
        //    return tc.Task;
        //}

        /// <summary>
        /// 从ViewModel打开一个子窗体
        /// 并且将数据传递过去，这次不借助ITangdaoWindow
        /// 子类不需要实现ITangdaoWindow，通过owner传递
        /// name是子窗体数据的名称
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static object RunChildWindowCallBackListAsync<TWindow>(this DaoViewModelBase viewModel, string name, Action<TWindow> action) where TWindow : Window, new()
        {
            //获取当前窗体 正在活动的窗体
            var activeWindow1 = Application.Current.MainWindow;
            var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            TaskCompletionSource<object> tc = new TaskCompletionSource<object>();

            //是从主窗体打开子窗体，然后子窗体选择列表的一项，将单个数据发送给主窗体
            Thread thread = new Thread(() =>
            {
                TWindow view = new TWindow();
                view.Owner = activeWindow;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(Selector.SelectedItemProperty);
                binding.Source = action;
                view.Closed += View_Closed;

                view.Show();

                //如果不加这段代码，窗体因为没有启动消息循环，一闪而过
                System.Windows.Threading.Dispatcher.Run();

                tc.SetResult(null);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            //新线程启动，将Task实例返回，接收await操作符
            return tc.Task;
        }

        /// <summary>
        /// 使用时，可以使用Wait等待，仅此打开一个窗体，也可以使用await异步打开
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <param name="button"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static Task RunChildWindowAsync<TWindow>(this Button button) where TWindow : Window, new()
        {
            TaskCompletionSource<object> tc = new TaskCompletionSource<object>();

            Thread thread = new Thread(() =>
            {
                TWindow view = new TWindow();
                view.Closed += View_Closed;
                view.Show();

                //如果不加这段代码，窗体因为没有启动消息循环，一闪而过
                System.Windows.Threading.Dispatcher.Run();

                tc.SetResult(null);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            //新线程启动，将Task实例返回，接收await操作符
            return tc.Task;
        }

        /// <summary>
        /// 当窗体关闭后结束消息循环
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void View_Closed(object sender, EventArgs e)
        {
            System.Windows.Threading.Dispatcher.ExitAllFrames();
        }
    }
}