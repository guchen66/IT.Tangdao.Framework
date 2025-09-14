using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework
{
    public abstract class TangdaoAppBase : Application//, IDataTemplateHost
    {
        private ITangdaoProvider _provider;

        public ITangdaoProvider TangdaoProvider { get => _provider; }

        private ITangdaoContainer _container;

        public ITangdaoContainer TangdaoContainer { get => _container; }

        //  private TangdaoDataTemplates _tangdaoDemplates;

        //  public TangdaoDataTemplates TangdaoDataTemplates => _tangdaoDemplates ?? (_tangdaoDemplates = new TangdaoDataTemplates());

        //   bool IDataTemplateHost.IsDataTemplatesInitialized => _tangdaoDemplates != null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _container = new TangdaoContainer();
            _provider = TangdaoContainer.Builder();
            Init();
        }

        protected TangdaoAppBase()
        {
            /* Container = new TangdaoContainer();
             _provider = Container.Builder();
             Init();*/
        }

        public virtual void Init()
        {
            InitServerLocator();
            InitIoc();
            InitWindow();
        }

        protected void InitWindow()
        {
            CreateWindow().Show();
        }

        protected void InitIoc()
        {
            ConfigureIOC(this.TangdaoContainer);
        }

        /// <summary>
        /// 启动主窗体
        /// </summary>
        /// <returns></returns>
        protected abstract Window CreateWindow();

        /// <summary>
        /// 使用IOC容器进行注册
        /// </summary>
        protected abstract void ConfigureIOC(ITangdaoContainer container);

        /// <summary>
        /// 初始化界面资源
        /// </summary>
        /// <param name="resourceDictionary"></param>
      //  protected abstract void InitResource(ResourceDictionary resourceDictionary);

        //注册服务定位器
        private void InitServerLocator()
        {
            ServerLocator.InitContainer(this.TangdaoContainer);
        }
    }
}