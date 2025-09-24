using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    public abstract class TangdaoModuleBase : ITangdaoModule
    {
        public virtual int Order => 0;
        public virtual bool Lazy => false;

        public abstract void RegisterServices(ITangdaoContainer container);

        public virtual void OnInitialized(ITangdaoProvider provider)
        { }
    }
}