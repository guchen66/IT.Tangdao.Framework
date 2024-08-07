using IT.Tangdao.Framework.DaoCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.DaoEvents
{
    public class DaoToastEvent:EventArgs
    {
        public string XmlPath {  get; set; }
        public string XmlData { get; set; }
        public string Name {  get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }=DateTime.Now;

        public void Usage()
        {
            DaoToast daoToast = new DaoToast();
            daoToast.Loaded+=new EventHandler<RoutedEventArgs>(daoToast.DaoToast_Loaded);
        }
    }
}
