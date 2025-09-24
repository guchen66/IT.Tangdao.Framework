using IT.Tangdao.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// “只写”容器：仅暴露注册能力，不暴露解析。
    /// 实现与解析侧完全隔离，符合接口隔离原则。
    /// </summary>
    public interface ITangdaoContainer
    {
        /// <summary>
        /// 注册一条服务映射；重复注册覆盖。
        /// </summary>
        void Register(IServiceEntry entry);

        /// <summary>
        /// 拿到内部注册表（只读），供 Builder 或 Visitor 使用。
        /// </summary>
        IServiceRegistry Registry { get; }

        /// <summary>
        /// 从当前只写容器生成只读解析器。
        /// </summary>
        ITangdaoProvider BuildProvider();

        /* 新增：仅供框架内部做延迟注册 */

        [EditorBrowsable(EditorBrowsableState.Never)]
        IList<Action<ITangdaoContainer>> LazyRegistrations { get; }
    }
}