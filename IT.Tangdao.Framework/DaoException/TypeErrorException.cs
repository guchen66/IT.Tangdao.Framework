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
}
