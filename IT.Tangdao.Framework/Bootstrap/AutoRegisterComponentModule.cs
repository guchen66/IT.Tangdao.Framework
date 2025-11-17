using IT.Tangdao.Framework.Component;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Alarms;
using IT.Tangdao.Framework.Ioc;
using System.Windows;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Enums;
using System.Windows.Threading;
using IT.Tangdao.Framework.Attributes;
using System.Reflection;
using IT.Tangdao.Framework.Threading;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Mvvm;

namespace IT.Tangdao.Framework.Bootstrap
{
    internal sealed class AutoRegisterComponentModule : TangdaoModuleBase
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(AutoRegisterComponentModule));

        public override void RegisterServices(ITangdaoContainer container)
        {
            TangdaoAutoRegistry.Register(container);
            Logger.WriteLocal("所有View注册成功");
        }

        public override void OnInitialized(ITangdaoProvider provider)
        {
        }
    }
}