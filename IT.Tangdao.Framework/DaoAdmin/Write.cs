using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public class Write : IWrite
    {
        public object WriteObject
        {
            get => _writeObject;
            set => _writeObject = value;
        }

        private object _writeObject;

        // 实现接口中的索引器
        public IWrite this[object writeObject]
        {
            get
            {
                _writeObject = writeObject;
                return this;
            }
        }

        public IWriteResult WriteObjectToXML<TTarget>()
        {
            return new IWriteResult("", true);
        }

        public IWriteResult WriteObjectToJson()
        {
            return new IWriteResult("", true);
        }

        public void Save()
        {
        }
    }
}