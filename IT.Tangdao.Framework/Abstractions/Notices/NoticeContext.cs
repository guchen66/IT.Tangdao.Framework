using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知上下文，包含通知的相关信息
    /// 用于在通知中介者和观察者之间传递数据
    /// </summary>
    public sealed class NoticeContext
    {
        /// <summary>
        /// 通知标签，用于标识通知的类型或来源
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 当前通知状态
        /// </summary>
        public bool CurrentState { get; set; }

        /// <summary>
        /// 通知产生的时间
        /// </summary>
        public DateTime CurrentTime { get; set; }

        /// <summary>
        /// 通知的唯一标识符
        /// </summary>
        public string NoticeId { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 通知的具体消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 通知的额外数据，可以是任意类型
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 通知的发送者
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// 通知的优先级
        /// </summary>
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;

        /// <summary>
        /// 初始化通知上下文实例
        /// </summary>
        public NoticeContext()
        {
            CurrentTime = DateTime.Now;
        }

        /// <summary>
        /// 初始化通知上下文实例
        /// </summary>
        /// <param name="tag">通知标签，用于标识通知的类型或来源</param>
        /// <param name="currentState">当前通知状态</param>
        public NoticeContext(string tag, bool currentState)
        {
            Tag = tag;
            CurrentState = currentState;
            CurrentTime = DateTime.Now;
        }

        /// <summary>
        /// 初始化通知上下文实例
        /// </summary>
        /// <param name="tag">通知标签</param>
        /// <param name="currentState">当前状态</param>
        /// <param name="message">通知消息</param>
        public NoticeContext(string tag, bool currentState, string message)
        {
            Tag = tag;
            CurrentState = currentState;
            CurrentTime = DateTime.Now;
            Message = message;
        }

        /// <summary>
        /// 初始化通知上下文实例
        /// </summary>
        /// <param name="tag">通知标签</param>
        /// <param name="currentState">当前状态</param>
        /// <param name="message">通知消息</param>
        /// <param name="data">额外数据</param>
        public NoticeContext(string tag, bool currentState, string message, object data)
        {
            Tag = tag;
            CurrentState = currentState;
            CurrentTime = DateTime.Now;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// 类型安全地获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>转换后的数据，转换失败则返回默认值</returns>
        public T GetData<T>()
        {
            return Data is T typedData ? typedData : default;
        }

        /// <summary>
        /// 类型安全地获取数据，如果数据为null则使用默认值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的数据，转换失败则返回默认值</returns>
        public T GetData<T>(T defaultValue)
        {
            return Data is T typedData ? typedData : defaultValue;
        }
    }
}