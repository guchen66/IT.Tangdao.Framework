using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 必需的配置项未提供或为空。
    /// </summary>
    public class ConfigurationMissingException : TangdaoException
    {
        public ConfigurationMissingException(string key)
            : base($"配置节点 '{key}' 缺失或值为空。") { }

        public ConfigurationMissingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}