using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 访问者模式：对 ServiceRegistry 中的每条记录做只读操作。
    /// 实现类无状态，遍历快照，绝不修改注册表。
    /// </summary>
    public interface IRegistryVisitor
    {
        void Visit(IServiceEntry entry);
    }
}