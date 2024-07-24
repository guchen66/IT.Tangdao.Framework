using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoCommon
{
    public class CommonContext
    {
        public Type ServiceType { get; set; }
        public Type[] ParameterTypes { get; set; }
        public object[] Parameters { get; set; } // 存储解析后的参数
        public Func<object[], object> Creator { get; set; } // 用于创建服务实例的委托，接受一个对象数组作为参数
        public Dictionary<Type, object> ResolvedParameters { get; set; } = new Dictionary<Type, object>();
        public Dictionary<Type, object> RegisterType { get; set; } = new Dictionary<Type, object>();
        public bool IsResolving { get; set; } // 标记是否正在解析该服务
    }

}
