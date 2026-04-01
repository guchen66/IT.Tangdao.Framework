using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using IT.Tangdao.Framework.Commands;
using IT.Tangdao.Framework.Mvvm;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ViewModelBaseExtension
    {
        /// <summary>
        /// 批量通知
        /// </summary>
        /// <param name="viewModelBase"></param>
        /// <param name="props"></param>
        public static void RaiseMulti(this ViewModelBase viewModelBase, params string[] props)
        {
            viewModelBase.RaiseMultiInternal(props);
        }

        /// <summary>
        /// 命令快速工厂
        /// </summary>
        /// <param name="viewModelBase"></param>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public static ICommand Cmd(this ViewModelBase viewModelBase, Action execute, Func<bool> canExecute = null)
        {
            return new TangdaoCommand(execute, canExecute);
        }

        /// <summary>
        /// 泛型命令快速工厂
        /// </summary>
        /// <param name="daoViewModelBase"></param>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public static ICommand Cmd<T>(this ViewModelBase viewModelBase, Action<T> execute, Func<T, bool> canExecute = null)
        {
            return new TangdaoCommand<T>(execute, canExecute);
        }

        /// <summary>
        /// 创建异步命令
        /// </summary>
        public static ICommand AsyncCmd(this ViewModelBase viewModelBase, Func<Task> execute, Func<bool> canExecute = null)
        {
            return new TangdaoAsyncCommand(execute, canExecute);
        }

        /// <summary>
        /// 创建泛型异步命令
        /// </summary>
        public static ICommand AsyncCmd<T>(this ViewModelBase viewModelBase, Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            return new TangdaoAsyncCommand<T>(execute, canExecute);
        }
    }
}