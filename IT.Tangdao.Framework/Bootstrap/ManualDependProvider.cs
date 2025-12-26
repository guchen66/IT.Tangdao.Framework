using IT.Tangdao.Framework.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Bootstrap
{
    internal sealed class ManualDependProvider
    {
        private static List<TangdaoComponentContext> _daoComponents = new List<TangdaoComponentContext>();

        internal static List<TangdaoComponentContext> CreateDependLinkList(Type componentType, object options = default)
        {
            // 根组件上下文
            var rootComponentContext = new TangdaoComponentContext
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
            var rootComponentContext = new TangdaoComponentContext
            {
                ComponentType = componentType,
                IsRoot = true
            };
            rootComponentContext.GetProperty<TangdaoComponentContext>(componentType);

            // 初始化组件依赖链
            var dependLinkList = new List<Type> { componentType };
            var componentContextLinkList = new List<TangdaoComponentContext> { rootComponentContext };

            // 创建组件依赖链
            //  CreateDependLinkList(componentType, ref dependLinkList, ref componentContextLinkList);

            return componentContextLinkList;
        }
    }
}