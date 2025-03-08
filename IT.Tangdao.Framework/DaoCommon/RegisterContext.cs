using IT.Tangdao.Framework.DaoEnums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoCommon
{
    public class RegisterContext
    {
        /// <summary>
        /// 注册的类
        /// </summary>
        public Type RegisterType { get; set; }

        /// <summary>
        /// 注册类构造器的参数
        /// </summary>
        public ParameterInfo[] ParameterInfos { get; set; }

        /// <summary>
        /// 存储接口和实现类的映射
        /// </summary>
        public ConcurrentDictionary<Type, Type> InterfaceToImplementationMapping { get; } = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// 存储接口和实现类的工厂
        /// </summary>
        public ConcurrentDictionary<Type, object> FactoryMapping { get; } = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// 标记是否正在解析该服务
        /// </summary>
        public bool IsResolving { get; set; }

        /// <summary>
        /// 生命周期
        /// </summary>
        public Lifecycle Lifecycle { get; set; }
    }
}