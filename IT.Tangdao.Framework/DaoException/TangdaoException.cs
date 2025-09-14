using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 唐刀框架异常基类。
    /// </summary>
    public class TangdaoException : Exception
    {
        public TangdaoException()
        { }

        public TangdaoException(string message) : base(message)
        {
        }

        public TangdaoException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
    [System.Runtime.Serialization.Constructor]
#endif

        protected TangdaoException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}