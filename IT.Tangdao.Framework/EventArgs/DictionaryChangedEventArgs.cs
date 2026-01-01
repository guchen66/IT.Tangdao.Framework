using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.EventArg
{
    /// <summary>
    /// 字典变化事件参数
    /// </summary>
    public class DictionaryChangedEventArgs<TKey, TValue> : EventArgs
    {
        /// <summary>
        /// 获取变化操作类型
        /// </summary>
        public DictionaryChangedAction Action { get; }

        /// <summary>
        /// 获取变化的键
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// 获取旧值
        /// </summary>
        public TValue OldValue { get; }

        /// <summary>
        /// 获取新值
        /// </summary>
        public TValue NewValue { get; }

        /// <summary>
        /// 初始化字典变化事件参数
        /// </summary>
        public DictionaryChangedEventArgs(DictionaryChangedAction action, TKey key, TValue oldValue = default, TValue newValue = default)
        {
            Action = action;
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}