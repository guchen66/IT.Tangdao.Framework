using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.Windows
{
    public class WindowAction
    {
        private IEventAggregator _eventAggregator;

        public WindowAction(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe<WinEventBase>(ExecuteAction);
        }

        private void ExecuteAction(WinEventBase @event)
        {
            Window window = ServiceLocator.Default.GetService(@event.WindowType) as Window; ;
            bool? result = window.ShowDialog();
            if (result == true)
            {
                @event.SucessAction?.Invoke();
            }
            else
            {
                @event.FailureAction?.Invoke();
            }
        }
    }
}