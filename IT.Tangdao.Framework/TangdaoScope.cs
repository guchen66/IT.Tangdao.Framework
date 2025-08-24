using IT.Tangdao.Framework.DaoMvvm;
using IT.Tangdao.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IT.Tangdao.Framework
{
    public class TangdaoScope : ITangdaoScope
    {
        internal TangdaoProvider _tangdaoProvider;

        public static object FromContainerType(Type type)
        {
            return ManualDependProvider.ResolveDependLinkList(type);
        }

        public object Resolve(Type type)
        {
            return _tangdaoProvider.Resolve(type);
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ServiceAppBase : Application
    {
        private IServiceProvider _container;

        // private IServiceCollection _service;

        public IServiceProvider ServiceContainer { get => _container; }

        //  public IServiceCollection Service { get => _service; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //  _service = new ServiceCollection();

            Init();
        }

        public virtual void Init()
        {
            InitServerLocator();
            InitIoc();
            InitProvider();

            InitWindow();
        }

        protected void InitWindow()
        {
            var viewModel = CreateWindow();
            var main = Build(viewModel) as Window;
            main.Show();
        }

        protected void InitIoc()
        {
            // ConfigureIOC(Service);
        }

        protected void InitProvider()
        {
            // _container = _service.BuildServiceProvider();
        }

        protected abstract DaoViewModelBase CreateWindow();

        //  protected abstract void ConfigureIOC(IServiceCollection service);

        private void InitServerLocator()
        {
            // ServiceCollectionLocator.InitContainer(Service);
        }

        public static Control Build(object data)
        {
            if (data is null)
                return null;

            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                var control = (Control)Activator.CreateInstance(type);
                control.DataContext = data;
                return control;
            }

            return new Control();
        }
    }

    public class ServiceCollectionLocator
    {
        public static IServiceProvider Current { get; set; }

        /* public static IServiceProvider InitContainer(IServiceCollection container)
         {
             Current = container.BuildServiceProvider();
             return Current;
         }*/
    }
}