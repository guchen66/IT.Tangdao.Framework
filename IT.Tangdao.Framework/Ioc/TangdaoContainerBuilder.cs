using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 默认建造者实现。
    /// </summary>
    public sealed class TangdaoContainerBuilder : ITangdaoContainerBuilder
    {
        // 1. 保存“容器构建完成后”要执行的回调
        private readonly List<Action<ITangdaoProvider>> _builtCallbacks = new List<Action<ITangdaoProvider>>();

        public ITangdaoContainer Container { get; }

        public TangdaoContainerBuilder()
        {
            Container = new TangdaoContainer(); // 内部已带空 Registry
        }

        public void ValidateDependencies()
        {
            Container.Registry.ValidateDependencies(); // 用前面写好的访问者
        }

        public ITangdaoContainer Build()
        {
            // 目前只是快照返回，后续可加锁防再写
            return Container;
        }

        // 2. 供框架内部注册初始化钩子（模块 OnInitialized）
        internal void AddBuiltCallback(Action<ITangdaoProvider> callback)
            => _builtCallbacks.Add(callback);

        // 3. 由 TangdaoApplication 在 BuildProvider() 后调用
        internal void RaiseBuilt(ITangdaoProvider provider)
        {
            var exceptions = new List<Exception>();
            foreach (var back in _builtCallbacks)
            {
                try
                {
                    back.Invoke(provider);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Count > 0)
                throw new AggregateException("模块初始化失败，见内部异常", exceptions);
        }
    }
}