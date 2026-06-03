using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    internal static class ModuleFinder
    {
        /// <summary>
        /// 要想生效，用户代码需要写[assembly: TangdaoModule(typeof(DemoModule))]
        /// </summary>
        /// <returns></returns>
        public static List<ITangdaoModule> SelectModules()
        {
            var list = new List<ITangdaoModule>();
            foreach (var asm in AssemblyExtension.GetModuleAssemblies())
            {
                foreach (var attr in asm.GetCustomAttributes<TangdaoModuleAttribute>())
                {
                    if (Activator.CreateInstance(attr.ModuleType) is ITangdaoModule module)
                        list.Add(module);
                }
            }
            return list;
        }

        /// <summary>
        /// 注册模块以及将初始化回调存在builder的委托
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="builder"></param>
        public static void RegisterModules(IReadOnlyList<ITangdaoModule> catalog, TangdaoContainerBuilder builder)
        {
            var eager = catalog.Where(m => !m.Lazy).OrderBy(m => m.Order);
            foreach (var m in eager)
            {
                m.RegisterServices(builder.Container);
                builder.AddBuiltCallback(provider => m.OnInitialized(provider));
            }

            // 懒加载模块：注册一个工厂，第一次解析时触发真实 RegisterServices
            // 延迟注册（只攒动作，不解析）
            foreach (var m in catalog.Where(m => m.Lazy))
            {
                var moduleCopy = m;
                builder.Container.RegisterLazyRegistration(c =>
                {
                    moduleCopy.RegisterServices(c);
                    builder.AddBuiltCallback(provider => m.OnInitialized(provider));
                });
            }
        }
    }
}