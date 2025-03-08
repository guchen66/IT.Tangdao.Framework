using IT.Tangdao.Framework.DaoAdmin.IServices;
using IT.Tangdao.Framework.DaoInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.DaoAdmin.Services
{
    public class NavigateService : INavigateService
    {
        private readonly ITangdaoProvider _provider;

        public NavigateService(ITangdaoProvider provider)
        {
            _provider = provider;
        }

        public void OnNavigatedTo(string viewModelName, ITangdaoParameter tangdaoParameter)
        {
            // 通过 ViewModel 名称获取对应的 ViewModel 类型
            var viewModelType = GetViewModelType(viewModelName);
            var viewType = GetViewType(viewModelName);
            if (viewModelType == null)
            {
                throw new InvalidOperationException($"ViewModel '{viewModelName}' not found.");
            }

            Window view = Activator.CreateInstance(viewType) as Window;

            // 通过 IContainer 解析 ViewModel 实例
            var viewModel = _provider.Resolve(viewModelType) as INavigationAware;

            if (view == null)
            {
                throw new InvalidOperationException($"View不是Window类型");
            }

            viewModel.OpenWindowExecute(tangdaoParameter);
            view.Show();
        }

        private Type GetViewType(string viewModelName)
        {
            // 根据 ViewModel 名称获取对应的 View 类型
            // 例如：MainViewModel -> MainView
            var viewName = viewModelName.Replace("ViewModel", "View");
            var assembly = Assembly.GetEntryAssembly();
            string assemblyName = assembly.GetName().Name;
            string typeName = $"{assemblyName}.Views.{viewName}";
            return assembly.GetType(typeName);
        }

        private Type GetViewModelType(string viewModelName)
        {
            var assembly = Assembly.GetEntryAssembly();
            // 获取程序集的简单名称
            string assemblyName = assembly.GetName().Name;
            string typeName = $"{assemblyName}.ViewModels.{viewModelName}";
            // 根据 ViewModel 名称获取对应的 ViewModel 类型

            return assembly.GetType(typeName);
        }
    }
}