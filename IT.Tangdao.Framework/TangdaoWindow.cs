using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace IT.Tangdao.Framework
{
    public class TangdaoWindow : Window, ITangdaoWindow, IComponentConnector
    {
        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }

        public void InitializeComponent()
        {
            throw new NotImplementedException();
        }
    }
}