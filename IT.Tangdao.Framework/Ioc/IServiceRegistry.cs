using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 线程安全的“只写一次/多次读”注册表。
    /// 建造阶段往里 Add，解析阶段任意线程查询。
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// 添加或覆盖一条注册信息。
        /// </summary>
        void Add(IServiceEntry entry);

        /// <summary>
        /// 查询指定服务类型的注册信息；不存在返回 null。
        /// </summary>
        IServiceEntry GetEntry(Type serviceType);

        /// <summary>
        /// 供访问者模式遍历快照，避免并发修改问题。
        /// </summary>
        IReadOnlyList<IServiceEntry> GetAllEntries();

        /// <summary>
        /// 新增：按 (ServiceType,Key) 存条目
        /// </summary>
        void AddKeyed(IServiceEntry entry, object key);

        /// <summary>
        /// 新增：按 (ServiceType,Key) 查条目
        /// </summary>
        IServiceEntry GetKeyedEntry(Type serviceType, object key);
    }
}