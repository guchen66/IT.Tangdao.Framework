using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Ioc;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 作用域策略：同一线程/同 Scope 内复用；无 Scope 时退化为 Transient。
    /// </summary>
    /// <summary>
    /// 作用域策略：同一线程/同 Scope 内复用；无 Scope 时退化为 Transient。
    /// </summary>
    public sealed class ScopedStrategy : ILifecycleStrategy
    {
        // 线程静态，支持异步流
        private static readonly AsyncLocal<ITangdaoScope> _currentScope = new AsyncLocal<ITangdaoScope>();

        /// <summary>
        /// 供框架设置当前作用域（由 CreateScope 扩展调用）。
        /// </summary>
        internal static void SetCurrent(ITangdaoScope scope) => _currentScope.Value = scope;

        public object CreateInstance(ServiceCreationContext context)
        {
            var scope = _currentScope.Value;
            if (scope == null) return context.Factory.Create(context.Entry); // 退化为瞬态

            // 从作用域缓存拿
            var cached = scope.ScopedProvider.GetService(context.Entry.ServiceType);
            if (cached != null) return cached;

            // 新建并跟踪
            var inst = context.Factory.Create(context.Entry);
            scope.TrackForDispose(inst);
            return inst;
        }
    }
}