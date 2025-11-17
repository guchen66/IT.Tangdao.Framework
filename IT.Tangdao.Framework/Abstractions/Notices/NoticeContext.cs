using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通知作用域上下文，不可变
    /// </summary>
    public sealed class NoticeContext
    {
        public string Key { get; }
        public object Parameter { get; }

        public NoticeContext(string key, object parameter = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Parameter = parameter;
        }

        // 便于字典比较
        public override bool Equals(object obj)
            => obj is NoticeContext other && Key == other.Key;

        public override int GetHashCode() => Key.GetHashCode();
    }
}