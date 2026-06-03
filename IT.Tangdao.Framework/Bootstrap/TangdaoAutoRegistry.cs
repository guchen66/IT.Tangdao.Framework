using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IT.Tangdao.Framework.Abstractions.Loggers;
using System.Reflection;
using IT.Tangdao.Framework.Infrastructure;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Bootstrap
{
    /// <summary>
    /// 自动注册所有视图
    /// </summary>
    internal sealed class TangdaoAutoRegistry
    {
        public static void Register(ITangdaoContainer tangdaoContainer)
        {
            var AttributeInfos = AttributeSelector.GetAttributeInfos<AutoRegisterAttribute>();
            Array.Sort(AttributeInfos, (a, b) => a.Attribute.CompareTo(b.Attribute));
            foreach (var info in AttributeInfos)
            {
                AutoRegisterAttribute registerAttribute = info.Attribute;
                switch (registerAttribute.Mode)
                {
                    case RegisterMode.Transient:
                        tangdaoContainer.RegisterTransient(info.Type);
                        break;

                    case RegisterMode.Singleton:
                        tangdaoContainer.RegisterSingleton(info.Type);
                        break;

                    case RegisterMode.Scoped:
                        tangdaoContainer.RegisterScoped(info.Type);
                        break;

                    default:
                        tangdaoContainer.RegisterSingleton(info.Type);
                        break;
                }
            }
        }
    }
}