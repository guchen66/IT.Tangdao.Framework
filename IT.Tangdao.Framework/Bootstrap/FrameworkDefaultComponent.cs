using IT.Tangdao.Framework.Component;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.FileAccessor;
using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Abstractions;

namespace IT.Tangdao.Framework.Bootstrap
{
    internal sealed class FrameworkDefaultComponent : ITangdaoContainerComponent
    {
        public void Load(ITangdaoContainer container, TangdaoComponentContext context)
        {
            // 框架级默认服务
            container.AddTangdaoSingleton<IContentReader, ContentReader>();
            container.AddTangdaoSingleton<IContentWriter, ContentWriter>();
            container.AddTangdaoSingleton<IFileLocator, FileLocator>();
            var loader = new TangdaoConfigLoader();
            // 2. 立即 Load 并塞进容器
            container.AddTangdaoSingleton(loader.Load());
            container.AddTangdaoSingleton<IDaoEventAggregator, DaoEventAggregator>();
            // 2. 默认通知器（用户可再注册覆盖）
        }
    }
}