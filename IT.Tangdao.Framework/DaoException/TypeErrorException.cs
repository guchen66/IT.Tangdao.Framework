using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IT.Tangdao.Framework.DaoException
{
   public class TypeErrorException : Exception
   {
        public TypeErrorException(string message) : base(message)
        {
            
        }
    }

    public class ControlTypeErrorException : TypeErrorException
    {
        public string Message { get; set; }

        public ControlTypeErrorException(string message) : base(message)
        {
            Message = message;
        }

        public static Control Create(string message)
        {
            return new Control();
        }
    }

    // 自定义异常类：缺少无参构造器
    public class MissingParameterlyConstructorException : Exception
    {
        public MissingParameterlyConstructorException(string message) : base(message)
        {
        }
    }

    // 自定义异常类：命名不规范
    public class ImproperNamingException : Exception
    {
        public ImproperNamingException(string message): base(message)
        {
        }
    }
}
