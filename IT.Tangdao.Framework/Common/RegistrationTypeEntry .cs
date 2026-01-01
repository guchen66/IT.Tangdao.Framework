using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Contracts;

namespace IT.Tangdao.Framework.Common
{
    /// <summary>
    /// 注册表，用于存储的类型信息
    /// </summary>
    public class RegistrationTypeEntry : IRegistrationTypeEntry
    {
        /// <summary>
        /// 注册Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 注册表键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 注册的类型
        /// </summary>
        public Type RegisterType { get; set; }

        /// <summary>
        /// 初始化通知注册表实例
        /// </summary>
        /// <param name="key">注册表键，用于标识类型</param>
        /// <param name="registerType">注册的类型</param>
        /// <exception cref="ArgumentNullException">当key或registerType为null时抛出</exception>
        public RegistrationTypeEntry(string key, Type registerType)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            RegisterType = registerType ?? throw new ArgumentNullException(nameof(registerType));
        }

        /// <summary>
        /// 初始化通知注册表实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registerType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RegistrationTypeEntry(int? id, Type registerType)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            RegisterType = registerType ?? throw new ArgumentNullException(nameof(registerType));
        }

        public bool IsNull() => false;   // 真对象永远非空
    }
}