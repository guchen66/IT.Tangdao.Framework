using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Configurations
{
    /// <summary>
    /// INI 文件配置集合
    /// </summary>
    public class IniConfigCollection : List<IniConfig>
    {
        /// <summary>
        /// 根据节名查找配置
        /// </summary>
        public IniConfig this[string section]
        {
            get => this.FirstOrDefault(x => x.Section.Equals(section, StringComparison.OrdinalIgnoreCase));
        }
    }
}