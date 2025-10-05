using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Interfaces;

namespace IT.Tangdao.Framework.Utilys
{
    public static class DaoSysinfo
    {
        public static string CurrentAppName => QueryableAppName();

        public static string QueryableAppName()
        {
            return System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
        }

        public class DesignTimeParent
        {
            public static bool IsDesignTime(IAddParent vm) =>
                DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
    }
}