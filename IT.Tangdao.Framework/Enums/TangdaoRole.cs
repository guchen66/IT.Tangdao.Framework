using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 本库内置的角色清单。仅用于库内权限检查，不与外部 IAM 系统强耦合。
    /// </summary>
    internal enum TangdaoRole
    {
        /// <summary>
        /// 普通调用方，只能读取公开数据。
        /// </summary>
        Reader,

        /// <summary>
        /// 可写入、更新自己范围内的数据。
        /// </summary>
        Writer,

        /// <summary>
        /// 管理其他用户或配置，不能分配系统级权限。
        /// </summary>
        Manager,

        /// <summary>
        /// 全局管理员，可操作所有资源及角色。
        /// </summary>
        Admin
    }
}