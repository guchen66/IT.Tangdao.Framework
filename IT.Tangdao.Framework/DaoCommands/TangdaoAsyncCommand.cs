using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IT.Tangdao.Framework.DaoCommands
{
    public class TangdaoAsyncCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;

        private readonly Func<bool> _canExecuteAsync;

        private bool _isExecuting = false;

        public TangdaoAsyncCommand(Func<Task> executeAsync, Func<bool> canExecuteAsync = null)
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
            return _canExecuteAsync == null || _canExecuteAsync() && !_isExecuting;
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter)) 
            {
                _isExecuting = true;
                await _executeAsync();
                _isExecuting=false;
            }
        }
    }
}
