using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDtos
{
    public class TangdaoResponse
    {
        public string Message { get; set; }

        public bool Status { get; set; }

        public object Result { get; set; }

        public TangdaoResponse(string message, bool status = false)
        {
            Message = message;
            Status = status;
        }

        public TangdaoResponse(bool status, object result)
        {
            Status = status;
            Result = result;
        }
    }

    public class TangdaoResponse<T>
    {
        public string Message { get; set; }

        public bool Status { get; set; }

        public T Result { get; set; }

        public TangdaoResponse(string message, bool status = false)
        {
            Message = message;
            Status = status;
        }

        public TangdaoResponse(bool status, T result)
        {
            Status = status;
            Result = result;
        }
    }
}
