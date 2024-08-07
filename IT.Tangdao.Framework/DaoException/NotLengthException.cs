using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    public class NotLengthException:Exception
    {
        public NotLengthException(string message):base(message)
        {
            
        }
    }
}
