using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    /// <summary>
    /// 自动装配的元数据，程序启动自动执行查询根目录下的配置文件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class AutoConfigureAttribute : Attribute
    {
        public int Order { get; }

        public AutoConfigureAttribute(int order) => Order = order;
    }
}