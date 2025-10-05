using IT.Tangdao.Framework.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using IT.Tangdao.Framework.Parameters.Infrastructure;
using IT.Tangdao.Framework.Abstractions.Loggers;

namespace IT.Tangdao.Framework.DaoEvents.Handlers
{
    /// <summary>
    /// 异常处理事件
    /// </summary>
    public class TangdaoExceptionHandler
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(TangdaoExceptionHandler));

        private bool _isShowed;

        /// <summary>
        /// 非UI线程未捕获异常处理事件例如自己创建一个子线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log(e.ExceptionObject as Exception);
        }

        /// <summary>
        /// UI线程未捕获异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log(e.Exception);
        }

        /// <summary>
        /// Task线程内未捕获异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log(e.Exception);
        }

        private void Log(Exception exception)
        {
            Logger.Fatal("An uncaught exception occurred", exception);

            if (_isShowed) return;

            _isShowed = true;
            switch (exception)
            {
                case NotImplementedException _:
                    MessageBox.Show(
                        "Sorry! The feature has NOT been IMPLEMENTED. Please wait for the next version. ",
                        "Fatal",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;

                case NotSupportedException _:
                    MessageBox.Show(
                        "Sorry! The feature has NOT been SUPPORTED. Please wait for the next version. ",
                        "Fatal",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;

                default:
                    var result = MessageBox.Show(
                        $"Sorry! An uncaught EXCEPTION occurred. {Environment.NewLine}" +
                        $"You can pack and send log files in %AppData%\\Ignite\\Logs to the developer. Thank you! {Environment.NewLine}{Environment.NewLine}" +
                        $"Do you want to open the Logs folder? ",
                        "Fatal",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error);

                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(LogPathConfig.Root);
                    }

                    break;
            }
            //  ProcessController.Restart(-1);
        }
    }
}