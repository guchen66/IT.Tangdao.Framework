using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Providers
{
    public interface IBindModelProvider
    {
    }

    public interface IBindModelProvider<T> : IBindModelProvider
    {
        T Default { get; set; }
    }
}