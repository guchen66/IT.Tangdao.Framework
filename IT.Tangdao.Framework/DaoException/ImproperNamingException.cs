using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 命名不符合框架规范。
    /// </summary>
    public class ImproperNamingException : TangdaoException
    {
        public ImproperNamingException(string name)
            : base($"名称 '{name}' 不符合唐刀命名规范。") { }

        public ImproperNamingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}