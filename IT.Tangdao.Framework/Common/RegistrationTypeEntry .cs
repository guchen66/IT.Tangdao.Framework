using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Abstractions;

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

        /// <summary>
        /// 将当前RegistrationTypeEntry转换为ITangdaoParameter实例
        /// </summary>
        /// <returns>包含当前类型注册信息的ITangdaoParameter实例</returns>
        public ITangdaoParameter ToTangdaoParameter()
        {
            var parameter = new TangdaoParameter();
            parameter.Add("RegistrationTypeEntry", this);
            return parameter;
        }

        /// <summary>
        /// 将当前RegistrationTypeEntry转换为ITangdaoParameter实例并存储到全局上下文
        /// </summary>
        /// <param name="name">存储的参数名称</param>
        /// <returns>包含当前类型注册信息的ITangdaoParameter实例</returns>
        public ITangdaoParameter ToTangdaoParameterAndSetGlobal(string name)
        {
            var parameter = ToTangdaoParameter();
            TangdaoContext.SetTangdaoParameter(name, parameter);
            return parameter;
        }

        /// <summary>
        /// 从ITangdaoParameter实例中提取IRegistrationTypeEntry
        /// </summary>
        /// <param name="parameter">ITangdaoParameter实例</param>
        /// <returns>提取的IRegistrationTypeEntry实例，如果不存在则返回NullTypeEntry.Instance</returns>
        public static IRegistrationTypeEntry FromTangdaoParameter(ITangdaoParameter parameter)
        {
            if (parameter == null)
                return NullTypeEntry.Instance;

            return parameter.Get<IRegistrationTypeEntry>("RegistrationTypeEntry") ?? NullTypeEntry.Instance;
        }

        /// <summary>
        /// 从全局上下文中获取指定名称的IRegistrationTypeEntry
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>获取的IRegistrationTypeEntry实例，如果不存在则返回NullTypeEntry.Instance</returns>
        public static IRegistrationTypeEntry FromGlobalContext(string name)
        {
            var parameter = TangdaoContext.GetTangdaoParameter(name);
            return FromTangdaoParameter(parameter);
        }

        public bool IsNull() => false;   // 真对象永远非空
    }
}