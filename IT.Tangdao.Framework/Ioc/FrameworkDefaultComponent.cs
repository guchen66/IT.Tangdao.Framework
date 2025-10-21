using IT.Tangdao.Framework.Component;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Alarms;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.Ioc
{
    internal sealed class FrameworkDefaultComponent : ITangdaoContainerComponent
    {
        public void Load(ITangdaoContainer container, DaoComponentContext context)
        {
            // 框架级默认服务
            container.AddTangdaoSingleton<IReadService, ReadService>();
            container.AddTangdaoSingleton<IWriteService, WriteService>();
            container.AddTangdaoSingleton<IAlarmService, AlarmService>();
            container.AddTangdaoSingleton<IDaoEventAggregator, DaoEventAggregator>();
            // 2. 默认通知器（用户可再注册覆盖）
            container.AddTangdaoTransient<IAlarmNotifier, AlarmPopupNotifier>();
        }
    }

    // FrameworkDefaultComponentModule.cs
    internal sealed class FrameworkDefaultComponentModule : TangdaoModuleBase
    {
        public override void RegisterServices(ITangdaoContainer container)
        {
            // 用内部 Component 机制完成注册
            container.RegisterComponent<FrameworkDefaultComponent>();
        }

        public override void OnInitialized(ITangdaoProvider provider)
        {
            // 3. 把默认通知器自动挂上事件流
            var service = provider.GetService<IAlarmService>();
            var notifier = provider.GetService<IAlarmNotifier>();
            service.Subscribe(notifier);
        }
    }
}