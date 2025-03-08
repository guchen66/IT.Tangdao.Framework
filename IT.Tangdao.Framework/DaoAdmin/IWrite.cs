using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public interface IWrite
    {
        IWriteResult WriteObjectToXML<TTarget>();

        IWriteResult WriteObjectToJson();

        void Save();

        IWrite this[object writeObject] { get; }
    }
}