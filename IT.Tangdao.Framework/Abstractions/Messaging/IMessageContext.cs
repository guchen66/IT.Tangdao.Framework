using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.Messaging
{
    public interface IMessageContext
    {
        /// <summary>
        /// 当前通知状态
        /// </summary>
        bool CurrentState { get; set; }

        /// <summary>
        /// 通知产生的时间
        /// </summary>
        DateTime CurrentTime { get; set; }

        /// <summary>
        /// 通知的唯一标识符
        /// </summary>
        string NoticeId { get; }

        /// <summary>
        /// 通知的具体消息内容
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 通知的额外数据，可以是任意类型
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// 通知的发送者
        /// </summary>
        object Sender { get; set; }

        /// <summary>
        /// 通知标签，用于标识通知的类型或来源
        /// </summary>
        IRegistrationTypeEntry RegistrationTypeEntry { get; set; }

        /// <summary>
        /// 通知的优先级
        /// </summary>
        TaskPriority Priority { get; set; }
    }
}