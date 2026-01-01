using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 表示字典变化的操作类型
    /// </summary>
    public enum DictionaryChangedAction
    {
        /// <summary>
        /// 添加键值对
        /// </summary>
        Add,

        /// <summary>
        /// 移除键值对
        /// </summary>
        Remove,

        /// <summary>
        /// 更新键值对
        /// </summary>
        Update,

        /// <summary>
        /// 清除字典
        /// </summary>
        Clear
    }
}