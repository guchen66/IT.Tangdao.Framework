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
using IT.Tangdao.Framework.Abstractions.Configurations;
using System.Windows.Threading;
using IT.Tangdao.Framework.Attributes;
using System.Reflection;
using IT.Tangdao.Framework.Threading;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Bootstrap
{
    internal class ViewModelFirstComponentModule : TangdaoModuleBase
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(ViewModelFirstComponentModule));

        public override void RegisterServices(ITangdaoContainer container)
        {
        }

        public override void OnInitialized(ITangdaoProvider provider)
        {
        }
    }
}