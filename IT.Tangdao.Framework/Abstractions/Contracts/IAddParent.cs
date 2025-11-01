using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Ioc;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 对ViewModel增强可用功能
    /// </summary>
    public interface IAddParent
    {
    }

    // 放在你的框架核心库，零依赖

    //public static class AutoViewModelTemplateInstaller
    //{
    //    public static ITangdaoProvider InstallViewTemplates(this ITangdaoProvider provider)
    //    {
    //        var registry = TangdaoApplication.Provider.GetService<IServiceRegistry>();
    //        foreach (var entry in registry.GetAllEntries())
    //        {
    //            var vmType = entry.ServiceType;
    //            if (!typeof(IViewModel).IsAssignableFrom(vmType)) continue;

    //            // 约定：命名空间相同，类名去掉 "Model" 就是 View
    //            var viewName = vmType.Name.Replace("Model", "");
    //            var viewType = vmType.Assembly.GetTypes()
    //                                  .FirstOrDefault(t => t.Name == viewName &&
    //                                                  typeof(FrameworkElement).IsAssignableFrom(t));
    //            if (viewType == null) continue;

    //            var template = new DataTemplate
    //            {
    //                DataType = vmType,
    //                VisualTree = new FrameworkElementFactory(viewType)
    //            };
    //            Application.Current.Resources.Add(template.DataType, template);
    //        }
    //        return provider;
    //    }
    //}
}