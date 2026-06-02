using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 默认建造者实现。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class TangdaoContainerBuilder
    {
        public ITangdaoContainer Container { get; }

        private static TangdaoContainerBuilder _current;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TangdaoContainerBuilder Current => _current ?? (_current = _lazyContainer?.Value);

        private static Lazy<TangdaoContainerBuilder> _lazyContainer;

        public static void SetContainerExtension(Func<TangdaoContainerBuilder> factory) => _lazyContainer = new Lazy<TangdaoContainerBuilder>(factory);

        public TangdaoContainerBuilder()
        {
            Container = new TangdaoContainer();
        }

        public void ValidateDependencies()
        {
            ContainerUtils.ValidateDependencies(Container); // 用前面写好的访问者
        }

        // 2. 供框架内部注册初始化钩子（模块 OnInitialized）
        internal void AddBuiltCallback(Action<ITangdaoProvider> callback)
        {
            ContainerUtils.AddBuiltCallback(callback);
        }

        // 3. 由 TangdaoApplication 在 BuildProvider() 后调用
        internal void RaiseBuilt(ITangdaoProvider provider)
        {
            ContainerUtils.RaiseBuilt(provider);
        }
    }
}