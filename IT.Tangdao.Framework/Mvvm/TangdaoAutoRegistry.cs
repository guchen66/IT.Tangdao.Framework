using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IT.Tangdao.Framework.Abstractions.Loggers;
using System.Reflection;
using IT.Tangdao.Framework.Infrastructure;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Mvvm
{
    /// <summary>
    /// 自动注册所有视图
    /// </summary>
    internal sealed class TangdaoAutoRegistry
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(TangdaoAutoRegistry));

        public static void Register(ITangdaoContainer tangdaoContainer)
        {
            var AttributeInfos = TangdaoAttributeSelector.GetAttributeInfos<AutoRegisterAttribute>();

            foreach (var item in AttributeInfos)
            {
                Logger.WriteLocal($"排序前：{item.Type.ToString()}");
            }
            Array.Sort(AttributeInfos, (a, b) => a.Attribute.CompareTo(b.Attribute));

            foreach (var item in AttributeInfos)
            {
                Logger.WriteLocal($"排序后：{item.Type.FullName}  (Order={item.Attribute.Order})");
            }
            foreach (var info in AttributeInfos)
            {
                AutoRegisterAttribute registerAttribute = info.Attribute;
                //Logger.WriteLocal($"准备第一个注册的{info.Type.ToString()}");
                switch (registerAttribute.Mode)
                {
                    case RegisterMode.Transient:
                        tangdaoContainer.AddTangdaoTransient(info.Type);
                        break;

                    case RegisterMode.Singleton:
                        tangdaoContainer.AddTangdaoSingleton(info.Type);
                        break;

                    case RegisterMode.Scoped:
                        tangdaoContainer.AddTangdaoScoped(info.Type);
                        break;

                    default:
                        tangdaoContainer.AddTangdaoSingleton(info.Type);
                        break;
                }
            }
        }
    }
}