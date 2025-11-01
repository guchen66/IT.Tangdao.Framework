using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Configurations;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// ITangdaoMenuItem 的默认实现。
    /// </summary>
    public class TangdaoMenuItem : ITangdaoMenuItem
    {
        /// <summary>配置 Id</summary>
        public int Id { get; set; }

        /// <summary>配置名称</summary>
        public string MenuName { get; set; } = string.Empty;

        /// <summary>配置值</summary>
        public string Value { get; set; }

        /// <summary>子级菜单集合，默认空列表</summary>
        public IList<ITangdaoMenuItem> Childs { get; set; } = new List<ITangdaoMenuItem>();

        /// <summary>
        /// 简易字典索引器，用于按需存储/读取额外配置。
        /// 实际存储在 <see cref="_extended"/> 中。
        /// </summary>
        public string this[string key]
        {
            get => _extended.TryGetValue(key, out var v) ? v : string.Empty;
            set => _extended[key] = value;
        }

        /// <summary>扩展字段字典</summary>
        private readonly Dictionary<string, string> _extended = new Dictionary<string, string>();
    }
}