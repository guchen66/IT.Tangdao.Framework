using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public class IReadResult
    {
        public string Value { get; set; }

        public bool Status { get; set; }

        public object Result { get; set; }

        public IReadResult()
        {
        }

        public IReadResult(string value, bool status = false)
        {
            Value = value;
            Status = status;
        }

        public IReadResult(bool status, object result)
        {
            Status = status;
            Result = result;
        }
    }

    public class IReadResult<T> : IReadResult
    {
        public new T Result
        {
            get => (T)base.Result;
            set => base.Result = value;
        }

        public IReadResult()
        {
        }

        public IReadResult(string value, bool status = false)
        {
            Value = value;
            Status = status;
        }

        public IReadResult(bool status, T result)
        {
            Status = status;
            Result = result; // 这里会调用基类的 object Result 属性
        }
    }
}