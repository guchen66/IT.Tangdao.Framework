using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDtos
{
    public class ReadOrWriteResult
    {
        public string Message { get; set; }

        public bool Status { get; set; }

        public object Result { get; set; }

        public ReadOrWriteResult(string message, bool status = false)
        {
            Message = message;
            Status = status;
        }

        public ReadOrWriteResult(bool status, object result)
        {
            Status = status;
            Result = result;
        }
    }
}
