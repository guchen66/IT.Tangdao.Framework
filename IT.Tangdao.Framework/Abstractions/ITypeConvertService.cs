using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public interface ITypeConvertService
    {
        T Converter<T>(string name) where T : class, new();
    }
}