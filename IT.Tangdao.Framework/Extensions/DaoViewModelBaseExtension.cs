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
using IT.Tangdao.Framework.DaoMvvm;

namespace IT.Tangdao.Framework.Extensions
{
    public static class DaoViewModelBaseExtension
    {
        /// <summary>
        /// 批量通知
        /// </summary>
        /// <param name="daoViewModelBase"></param>
        /// <param name="props"></param>
        public static void RaiseMulti(this DaoViewModelBase daoViewModelBase, params string[] props)
        {
            daoViewModelBase.RaiseMultiInternal(props);
        }

        /// <summary>
        /// 命令快速工厂
        /// </summary>
        /// <param name="daoViewModelBase"></param>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public static ICommand Cmd(this DaoViewModelBase daoViewModelBase, Action execute, Func<bool> canExecute = null)
        {
            return new TangdaoCommand(execute, canExecute);
        }
    }
}