using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知注册表，用于存储通知观察者的类型信息
    /// </summary>
    public class NoticeRegistry
    {
        /// <summary>
        /// 注册表键，用于标识观察者类型
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// 注册的观察者类型
        /// </summary>
        public readonly Type RegisterType;

        /// <summary>
        /// 初始化通知注册表实例
        /// </summary>
        /// <param name="key">注册表键，用于标识观察者类型</param>
        /// <param name="registerType">注册的观察者类型</param>
        /// <exception cref="ArgumentNullException">当key或registerType为null时抛出</exception>
        public NoticeRegistry(string key, Type registerType)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            RegisterType = registerType ?? throw new ArgumentNullException(nameof(registerType));
            // 验证类型必须实现INoticeObserver接口
            if (!typeof(INoticeObserver).IsAssignableFrom(registerType))
                throw new InvalidOperationException(
                    $"类型 '{registerType.FullName}' 未实现 {nameof(INoticeObserver)} 接口，无法注册。");
        }
    }
}