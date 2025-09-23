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
    }
}