using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IT.Tangdao.Framework.Commands
{
    public abstract class MinidaoCommand : ICommand
    {
        public static MinidaoCommand Create(Action cb) => new MinidaoCommand<object>(_ => cb());

        public static MinidaoCommand Create<T>(Action<T> cb) => new MinidaoCommand<T>(cb);

        public static MinidaoCommand CreateFromTask(Func<Task> cb) => new MinidaoCommand<object>(_ => cb());

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);

        public abstract event EventHandler CanExecuteChanged;
    }
}