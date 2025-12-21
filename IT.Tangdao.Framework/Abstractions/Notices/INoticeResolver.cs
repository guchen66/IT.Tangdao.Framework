using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知观察者解析器接口，用于创建和管理通知观察者实例
    /// </summary>
    public interface INoticeResolver
    {
        /// <summary>
        /// 根据通知注册表创建通知观察者实例
        /// </summary>
        /// <param name="noticeRegistry">通知注册表，包含观察者类型信息</param>
        /// <returns>创建的通知观察者实例</returns>
        INoticeObserver CreateObserver(RegistrationTypeEntry  noticeRegistry);

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