using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure.Configurations
{
    /// <summary>
    /// INI 文件配置项
    /// </summary>
    public class IniConfig
    {
        /// <summary>
        /// 配置节标题（如 [SectionName]）
        /// </summary>
        public string Section { get; set; } = string.Empty;

        /// <summary>
        /// 键值对字典
        /// </summary>
        public Dictionary<string, string> KeyValues { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 索引器，方便访问键值
        /// </summary>
        public string this[string key]
        {
            get => KeyValues.TryGetValue(key, out var value) ? value : string.Empty;
            set => KeyValues[key] = value;
        }
    }
}