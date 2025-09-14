using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 表示文件系统节点的类型。
    /// </summary>
    public enum PathKind
    {
        /// <summary>
        /// 文件不存在
        /// </summary>
        None,

        /// <summary>
        /// 文件
        /// </summary>
        File,

        /// <summary>
        /// 目录（文件夹）
        /// </summary>
        Directory
    }
}