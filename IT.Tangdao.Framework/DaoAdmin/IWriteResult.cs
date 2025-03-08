using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public class IWriteResult
    {
        public string Message { get; set; }

        public bool Status { get; set; }

        public object Result { get; set; }

        public IWriteResult()
        {
        }

        public IWriteResult(string message, bool status = false)
        {
            Message = message;
            Status = status;
        }

        public IWriteResult(bool status, object result)
        {
            Status = status;
            Result = result;
        }
    }
}