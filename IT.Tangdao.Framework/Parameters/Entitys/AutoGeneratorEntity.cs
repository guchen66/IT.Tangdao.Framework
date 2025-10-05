using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Parameters.Entitys
{
    /// <summary>
    /// 与数据库相关，描述实体类是否自动生成
    /// </summary>
    public class AutoGeneratorEntity
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否自动生成
        /// </summary>
        public bool IsAuto { get; set; }

        /// <summary>
        /// 是否生成种子数据
        /// </summary>
        public bool IsSeedData { get; set; }
    }
}