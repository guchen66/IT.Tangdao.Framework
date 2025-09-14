using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.DaoException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IT.Tangdao.Framework.Extensions;
namespace IT.Tangdao.Framework.DaoMvvm
{
    /// <summary>
    /// 视图定位器
    /// 用来全局定位MVVM模式
    /// </summary>
    public class ViewToViewModelLocator
    {
        public static ITangdaoContainer Build(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {               
                var name = type.FullName.Replace("ViewModel", "View");
                
                var viewType = Type.GetType(name);
               // viewType.get();
                if (viewType.IsSubclassOf(typeof(UserControl)))
                {
                    var view = (UserControl)Activator.CreateInstance(viewType);
                    view.DataContext = viewType;
                }
                else if (viewType.IsSubclassOf(typeof(Window)))
                {
                    var view = (Window)Activator.CreateInstance(viewType);
                    view.DataContext = viewType;
                }
                //var view = (Window)Activator.CreateInstance(viewType);
                var propertyInfo = viewType.GetProperty("DataContext");
            }

            return new TangdaoContainer();
        }

        public static void SetViewToViewModel(string viewModel)
        {

        }
        /* public static TType Build<TType>(object data)
         {
             var name = data.GetType().FullName.Replace("ViewModel", "View");
             var viewType = Type.GetType(name);

             if (viewType != null)
             {
                 if (viewType.IsSubclassOf(typeof(UserControl)))
                 {
                     var view = (UserControl)Activator.CreateInstance(viewType);
                     view.DataContext = data;
                     return (TType)view;
                 } 
                 else if (viewType.IsSubclassOf(typeof(Window)))
                 {
                     MainView mainView=new MainView();
                     var view = TryGetCurrentType(viewType);
                     view.DataContext = data;
                     return view;
                 }
             }
             else
             {
                 return ControlTypeErrorException.Create("未设定窗体的无参构造器");
             }
         }

         public static TType TryGetCurrentType<TType>(TType type)
         {
            return (TType)Activator.CreateInstance(typeof(TType));
         }*/
    }
}
