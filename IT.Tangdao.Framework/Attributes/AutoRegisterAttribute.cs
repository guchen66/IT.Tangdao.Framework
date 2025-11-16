using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;

namespace IT.Tangdao.Framework.Attributes
{
    /// <summary>
    /// 容器自动注册元数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutoRegisterAttribute : Attribute, IComparable<AutoRegisterAttribute>
    {
        public RegisterMode Mode { get; set; }

        public int Order { get; set; }

        public int CompareTo(AutoRegisterAttribute other)
        {
            if (other is null) return 1;               // null 永远排后面
            int c = Order.CompareTo(other.Order);      // 小 Order 在前
            if (c != 0) return c;
            // 同一 Order 时再按类型全名排，保证确定性
            return string.Compare(this.GetType().FullName, other.GetType().FullName, StringComparison.Ordinal);
        }
    }
}