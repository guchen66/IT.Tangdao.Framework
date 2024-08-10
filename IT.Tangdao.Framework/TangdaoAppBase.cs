using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework
{
    public abstract class TangdaoAppBase:Application
    {
        private ITangdaoProvider _provider {  get; set; }

        public ITangdaoProvider Provider => _provider;

        private ITangdaoContainer _container;

        public ITangdaoContainer Container { get; }

        protected TangdaoAppBase()
        {
            Container = new TangdaoContainer();
            _provider = Container.Builder();
            Init();
        }

        public virtual void Init()
        {
            InitWindow();
            InitIoc();
        }

        protected void InitWindow()
        {
            CreateWindow().Show();
        }
        protected void InitIoc()
        {
            ConfigureIOC();
        }

        /// <summary>
        /// 启动主窗体
        /// </summary>
        /// <returns></returns>
        protected abstract Window CreateWindow();

        /// <summary>
        /// 使用IOC容器进行注册
        /// </summary>
        protected abstract void ConfigureIOC();

        /// <summary>
        /// 初始化界面资源
        /// </summary>
        /// <param name="resourceDictionary"></param>
      //  protected abstract void InitResource(ResourceDictionary resourceDictionary);

    }
}
