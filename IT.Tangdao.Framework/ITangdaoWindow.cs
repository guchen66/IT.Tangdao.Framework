using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework
{
    public interface ITangdaoWindow
    {
        object Content { get; set; }

        Window Owner { get; set; }

        object DataContext { get; set; }

        //IDialogResult Result { get; set; }

        event RoutedEventHandler Loaded;

        event EventHandler Closed;

        event CancelEventHandler Closing;

        void Close();

        void Show();

        bool? ShowDialog();
    }
}