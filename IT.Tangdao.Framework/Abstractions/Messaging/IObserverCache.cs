using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IT.Tangdao.Framework.Abstractions.Contracts;

namespace IT.Tangdao.Framework.Abstractions.Messaging
{
    /// <summary>
    /// 观察者缓存接口，用于创建和管理观察者实例的缓存
    /// </summary>
    public interface IObserverCache
    {
        /// <summary>
        /// 根据消息注册表创建消息观察者实例
        /// </summary>
        /// <param name="noticeRegistry">消息注册表，包含观察者类型信息</param>
        /// <returns>创建的消息观察者实例</returns>
        IMessageObserver CreateObserver(IRegistrationTypeEntry noticeRegistry);

        /// <summary>
        /// 清除观察者实例缓存
        /// </summary>
        void ClearCache();

        /// <summary>
        /// 从缓存中移除指定类型的观察者实例
        /// </summary>
        /// <param name="observerType">观察者类型</param>
        /// <returns>如果移除成功则返回true，否则返回false</returns>
        bool RemoveFromCache(Type observerType);
    }
}