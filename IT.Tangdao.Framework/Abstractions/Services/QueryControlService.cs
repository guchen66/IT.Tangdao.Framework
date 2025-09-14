using IT.Tangdao.Framework.Abstractions.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IT.Tangdao.Framework.Abstractions.Services
{
    public class QueryControlService : IQueryControlService
    {
        public void QueryableLayoutControl(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox textBox)
                {
                  //  textBox.IsReadOnly = isReadOnly;
                }
                else
                {
                   // QueryableLayoutControl(child, isReadOnly);
                }
            }
        }

       /* private IEnumerable<Panel> Children
        {
            get
            {
                if (AssociatedObject is Panel panel)
                    return panel.Children.OfType<Button>();

                else if (AssociatedObject is ItemsControl control)
                    return control.Items.OfType<Button>();

                return Enumerable.Empty<Button>();
            }
        }*/
    }
}
