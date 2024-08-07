using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IT.Tangdao.Framework.DaoCommands
{
    public sealed class MinidaoCommand<T> : MinidaoCommand, ICommand
    {
        private readonly Action<T> _cb;
        private readonly Func<T, Task> _acb;
        private bool _busy;

        public MinidaoCommand(Action<T> cb)
        {
            _cb = cb;
        }

        public MinidaoCommand(Func<T, Task> cb)
        {
            _acb = cb;
        }

        private bool Busy
        {
            get => _busy;
            set
            {
                _busy = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public override event EventHandler CanExecuteChanged;

        public override bool CanExecute(object parameter) => !_busy;

        public override async void Execute(object parameter)
        {
            if (Busy)
                return;
            try
            {
                Busy = true;
                if (_cb != null)
                    _cb((T)parameter);
                else
                    await _acb((T)parameter);
            }
            finally
            {
                Busy = false;
            }
        }
    }

}
