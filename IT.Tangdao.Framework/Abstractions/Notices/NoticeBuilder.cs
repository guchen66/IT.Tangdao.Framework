using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Common;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知构建器，用于创建和注册通知观察者
    /// 提供流畅的API进行观察者注册
    /// </summary>
    public sealed class NoticeBuilder
    {
        /// <summary>
        /// 通知中介者实例，用于注册观察者
        /// </summary>
        private readonly NoticeMediator _mediator;

        /// <summary>
        /// 通知解析器实例，用于创建观察者
        /// </summary>
        private readonly INoticeResolver _resolver;

        /// <summary>
        /// 初始化通知构建器实例
        /// </summary>
        /// <param name="mediator">通知中介者实例</param>
        /// <param name="resolver">通知解析器实例</param>
        internal NoticeBuilder(NoticeMediator mediator, INoticeResolver resolver)
        {
            _mediator = mediator;
            _resolver = resolver;
        }

        /// <summary>
        /// 一键创建并注册指定类型的观察者
        /// </summary>
        /// <param name="type">观察者类型，必须实现INoticeObserver接口</param>
        /// <returns>当前NoticeBuilder实例，用于链式调用</returns>
        /// <exception cref="ArgumentNullException">当type为null时抛出</exception>
        /// <exception cref="InvalidOperationException">当type未实现INoticeObserver接口或无法创建实例时抛出</exception>
        public NoticeBuilder Add(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            // 1. 运行时防呆：必须实现 INoticeObserver
            if (!typeof(INoticeObserver).IsAssignableFrom(type))
                throw new InvalidOperationException(
                    $"类型 '{type.FullName}' 未实现 {nameof(INoticeObserver)} 接口，无法注册。");

            var registry = new RegistrationTypeEntry(type.Name, type);
            var obs = _resolver.CreateObserver(registry);

            // 2. 工厂也返回 null → 抛更明确的异常
            if (obs == null)
                throw new InvalidOperationException(
                    $"容器未注册类型 '{type.FullName}'，或注册失败。");

            _mediator.Register(registry.Key, obs);
            return this;
        }

        /// <summary>
        /// 一键创建并注册指定泛型类型的观察者（泛型版本）
        /// </summary>
        /// <typeparam name="T">观察者类型，必须实现INoticeObserver接口</typeparam>
        /// <returns>当前NoticeBuilder实例，用于链式调用</returns>
        /// <exception cref="InvalidOperationException">当无法创建实例时抛出</exception>
        public NoticeBuilder Add<T>() where T : INoticeObserver
        {
            var type = typeof(T);
            var registry = new RegistrationTypeEntry(type.Name, type);
            var obs = _resolver.CreateObserver(registry);

            // 工厂返回 null → 抛更明确的异常
            if (obs == null)
                throw new InvalidOperationException(
                    $"容器未注册类型 '{type.FullName}'，或注册失败。");

            _mediator.Register(registry.Key, obs);
            return this;
        }
    }
}