using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IT.Tangdao.Framework.Commands;

namespace IT.Tangdao.Framework.Helpers
{
    public class DragMoveHelper
    {
        public static DragMoveHelper Instance { get; } = new DragMoveHelper();

        /// <summary>
        /// 关键：公开一个 ICommand，而不是方法
        /// </summary>
        /// <returns></returns>
        public ICommand CreateDragMoveCommand()
        {
            return new TangdaoCommand(() =>
                Application.Current.Windows.OfType<Window>()
                           .SingleOrDefault(w => w.IsActive)?.DragMove());
        }
    }
}