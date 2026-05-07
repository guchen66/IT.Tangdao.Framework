using IT.Tangdao.Framework.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Windows
{
    public class EmptySignGuard<TContext> : EmptySignGuard where TContext : GuardContext
    {
        protected TContext Context { get; private set; }

        // 框架内部调用此方法注入上下文
        internal void SetContext(TContext context) => Context = context;

        // 如果需要，可以重写 Validate 使用 Context
        public override bool Validate() => base.Validate();
    }
}