using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IT.Tangdao.Framework.DaoCommands
{
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T,Task> _executeAsync;

        private readonly Func<T,bool> _canExecuteAsync;

        private bool _isExecuting = false;

        public AsyncRelayCommand(Func<T,Task> executeAsync, Func<T,bool> canExecuteAsync = null)
        {
            this._executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            this._canExecuteAsync = canExecuteAsync;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteAsync == null || _canExecuteAsync((T)parameter) && !_isExecuting;
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _isExecuting = true;
                await _executeAsync((T)parameter);
                _isExecuting = false;
            }
        }
    }
}
