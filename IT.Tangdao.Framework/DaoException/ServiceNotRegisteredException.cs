using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 请求的服务尚未在容器注册。
    /// </summary>
    public class ServiceNotRegisteredException : TangdaoException
    {
        public ServiceNotRegisteredException(Type serviceType)
            : base($"服务 '{serviceType.FullName}' 未注册。") { }

        public ServiceNotRegisteredException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}