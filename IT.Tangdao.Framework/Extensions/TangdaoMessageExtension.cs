using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Abstractions;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoMessageExtension
    {
        public static void Show(this Window window, ITangdaoParameter tangdaoParameter)
        {
            //var viewModel = window.DataContext;
            //if (viewModel is ITangdaoMessage message)
            //{
            //    message.Response(tangdaoParameter);
            //}
            //if (window == null || !window.IsLoaded)
            //{
            //    window.Show();
            //}
        }
    }
}