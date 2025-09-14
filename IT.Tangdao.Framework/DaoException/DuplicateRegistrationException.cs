using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 试图重复注册同一服务。
    /// </summary>
    public class DuplicateRegistrationException : TangdaoException
    {
        public DuplicateRegistrationException(Type serviceType)
            : base($"服务 '{serviceType.FullName}' 已注册，不能重复添加。") { }

        public DuplicateRegistrationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}