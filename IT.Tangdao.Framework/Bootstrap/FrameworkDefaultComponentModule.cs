using IT.Tangdao.Framework.Component;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Ioc;

namespace IT.Tangdao.Framework.Bootstrap
{
    internal sealed class FrameworkDefaultComponentModule : TangdaoModuleBase
    {
        public override void RegisterServices(ITangdaoContainer container)
        {
            // 用内部 Component 机制完成注册
            container.RegisterComponent<FrameworkDefaultComponent>();
        }

        public override void OnInitialized(ITangdaoProvider provider)
        {
        }
    }
}