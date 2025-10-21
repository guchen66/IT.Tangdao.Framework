using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 用于配置菜单
    /// </summary>
    public interface ITangdaoMenuItem
    {
        /// <summary>
        /// 配置Id
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        string MenuName { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        IList<ITangdaoMenuItem> Childs { get; set; }

        /// <summary>
        /// 获取或设置配置值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string this[string key] { get; set; }
    }
}