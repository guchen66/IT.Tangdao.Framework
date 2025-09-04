using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDtos.Globals
{
    public class DaoModel<T>
    {
        public static DaoModel<T> Instance;

        public static DaoModel<T> CreateInstance()
        {
            return Instance = new Lazy<DaoModel<T>>(() => new DaoModel<T>()).Value;
        }
    }
}