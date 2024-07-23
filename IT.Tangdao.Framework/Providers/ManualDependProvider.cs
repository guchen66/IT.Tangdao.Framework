using IT.Tangdao.Framework.DaoComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Providers
{
    internal class ManualDependProvider
    {
        private static List<DaoComponentContext> _daoComponents = new List<DaoComponentContext>();

        internal static List<DaoComponentContext> CreateDependLinkList(Type componentType, object options = default)
        {
            // 根组件上下文
            var rootComponentContext = new DaoComponentContext
            {
                ComponentType = componentType,
                IsRoot = true
            };
            rootComponentContext.SetProperty(componentType, options);

            // 初始化组件依赖链
            var dependLinkList = new List<Type> { componentType };
            _daoComponents.Add(rootComponentContext); 

            // 创建组件依赖链
            //  CreateDependLinkList(componentType, ref dependLinkList, ref componentContextLinkList);

            return _daoComponents;
        }

        internal static object ResolveDependLinkList(Type componentType, object options = default)
        {
            // 根组件上下文
            var rootComponentContext = new DaoComponentContext
            {
                ComponentType = componentType,
                IsRoot = true
            };
            rootComponentContext.GetProperty<DaoComponentContext>(componentType);

            // 初始化组件依赖链
            var dependLinkList = new List<Type> { componentType };
            var componentContextLinkList = new List<DaoComponentContext> { rootComponentContext };

            // 创建组件依赖链
            //  CreateDependLinkList(componentType, ref dependLinkList, ref componentContextLinkList);

            return componentContextLinkList;
        }
    }
}
