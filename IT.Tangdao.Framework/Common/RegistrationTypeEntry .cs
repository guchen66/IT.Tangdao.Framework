using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common
{
    /// <summary>
    /// 注册表，用于存储的类型信息
    /// </summary>
    public class RegistrationTypeEntry 
    {
        /// <summary>
        /// 注册表键
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// 注册的类型
        /// </summary>
        public readonly Type RegisterType;

        /// <summary>
        /// 初始化通知注册表实例
        /// </summary>
        /// <param name="key">注册表键，用于标识类型</param>
        /// <param name="registerType">注册的类型</param>
        /// <exception cref="ArgumentNullException">当key或registerType为null时抛出</exception>
        public RegistrationTypeEntry (string key, Type registerType)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            RegisterType = registerType ?? throw new ArgumentNullException(nameof(registerType));
        }
    }
}