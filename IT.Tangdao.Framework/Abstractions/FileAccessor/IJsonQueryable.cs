using IT.Tangdao.Framework.Abstractions.Results;
using System.Collections.Generic;
using System.Windows.Documents;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IJsonQueryable
    {
        ResponseResult SelectValue(string key);

        ResponseResult<IEnumerable<dynamic>> SelectKeys();

        ResponseResult<IEnumerable<dynamic>> SelectValues();

        ResponseResult<IEnumerable<T>> SelectObjects<T>() where T : new();
    }
}