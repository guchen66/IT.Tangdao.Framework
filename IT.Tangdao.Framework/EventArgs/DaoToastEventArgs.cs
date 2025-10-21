using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.EventArg
{
    public class DaoToastEventArgs : EventArgs
    {
        public string XmlPath { get; set; }
        public string XmlData { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;

        public static void Usage()
        {
        }
    }
}