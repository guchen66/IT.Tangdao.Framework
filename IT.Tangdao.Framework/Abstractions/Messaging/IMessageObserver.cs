using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.Abstractions.Messaging
{
    /// <summary>
    /// 消息观察者接口
    /// </summary>
    public interface IMessageObserver
    {
        /// <summary>
        /// 获取或设置观察者的标题/名称
        /// 可用于标识观察者，便于日志记录和调试
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 是否接收消息
        /// </summary>
        bool IsReceive { get; set; }

        /// <summary>
        /// 处理接收到的消息
        /// 当消息被允许传递时（IsReceive = true），此方法会被调用
        /// </summary>
        /// <param name="context">消息上下文</param>
        void UpdateMessage(MessageContext context);

        /// <summary>
        /// 消息拦截事件
        /// 在消息发送给 UpdateMessage 方法之前触发
        /// 观察者可以通过此事件：
        /// 1. 检查消息内容并决定是否接收（通过 IsReceive 属性）
        /// 2. 修改消息上下文（通过 Context 属性）
        /// 3. 执行预处理逻辑（如日志记录、权限检查等）
        /// </summary>
        EventHandler<MessageEventArgs> MessageIntercepted { get; set; }
    }
}